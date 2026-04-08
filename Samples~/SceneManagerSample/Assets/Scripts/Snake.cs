using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GameplayMechanicsUMFOSS.Core;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Grid-based snake with smooth interpolated visuals between cells.
    /// Eats apples to grow + score, dies on bombs / walls / self.
    /// Listens to InputLockEvent so it freezes during scene transitions.
    /// </summary>
    public class Snake : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private float cellSize = 0.4f;
        [SerializeField] private Vector2Int gridSize = new Vector2Int(32, 18);
        [SerializeField] private float moveInterval = 0.12f;

        [Header("Visuals")]
        [SerializeField] private Sprite segmentSprite;   // body — typically rounded square
        [SerializeField] private Sprite circleSprite;    // round corner segments + eyes
        [SerializeField] private Sprite tongueSprite;    // small rectangle for tongue
        [SerializeField] private Color headColor = new Color(0.5f, 1f, 0.45f);
        [SerializeField] private Color tailColor = new Color(0.15f, 0.55f, 0.30f);
        [SerializeField] private string pauseSceneName = "PauseMenu";

        [Header("Snake Shape")]
        // All segments are the same size. Scale > 1 makes the squares overlap so
        // the snake reads as a continuous shape — and at turns the diagonal overlap
        // is large enough that the corner cell stays visually connected, eliminating
        // the "visual friction" you see when small squares pivot around a turn.
        [SerializeField] private float bodyScaleMul = 1.22f;

        private readonly List<Transform> segments = new List<Transform>();
        private readonly List<Vector2Int> cells = new List<Vector2Int>();
        private readonly List<Vector3> cellWorldPositions = new List<Vector3>();
        private readonly List<Vector3> previousWorldPositions = new List<Vector3>();

        private Vector2Int direction = Vector2Int.right;
        private Vector2Int queuedDirection = Vector2Int.right;
        private float moveTimer;
        private float interpT; // 0..1 between previous and current
        private bool inputLocked;
        private bool dead;
        private int pendingGrowth;

        private void Awake()
        {
            // Initial 3-cell snake at center
            int cx = gridSize.x / 2;
            int cy = gridSize.y / 2;
            for (int i = 0; i < 3; i++)
            {
                var c = new Vector2Int(cx - i, cy);
                cells.Add(c);
                cellWorldPositions.Add(CellToWorld(c));
                previousWorldPositions.Add(CellToWorld(c));
                CreateSegment();
            }
            UpdateSegmentVisuals();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<InputLockEvent>(OnInputLock);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InputLockEvent>(OnInputLock);
        }

        private void OnInputLock(InputLockEvent e) => inputLocked = e.locked;

        public void SetMoveInterval(float interval) => moveInterval = interval;
        public Vector2Int GridSize => gridSize;
        public float CellSize => cellSize;
        public bool OccupiesCell(Vector2Int c) => cells.Contains(c);

        public Vector3 CellToWorld(Vector2Int c)
        {
            float x = (c.x - gridSize.x * 0.5f + 0.5f) * cellSize;
            float y = (c.y - gridSize.y * 0.5f + 0.5f) * cellSize;
            return new Vector3(x, y, 0f);
        }

        private void Update()
        {
            if (dead) return;

            if (!inputLocked)
            {
                // Read input
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.escapeKey.wasPressedThisFrame && SceneManager_UMFOSS.Instance != null)
                    {
                        SceneManager_UMFOSS.Instance.Push(pauseSceneName);
                        return;
                    }
                    if ((kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) && direction.y == 0)    queuedDirection = new Vector2Int(0, 1);
                    if ((kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) && direction.y == 0)  queuedDirection = new Vector2Int(0, -1);
                    if ((kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) && direction.x == 0)  queuedDirection = new Vector2Int(-1, 0);
                    if ((kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) && direction.x == 0) queuedDirection = new Vector2Int(1, 0);
                }

                // Tick
                moveTimer += Time.deltaTime;
                interpT = Mathf.Clamp01(moveTimer / moveInterval);

                // Smooth interpolated visual
                for (int i = 0; i < segments.Count; i++)
                {
                    segments[i].position = Vector3.Lerp(previousWorldPositions[i], cellWorldPositions[i], interpT);
                }

                if (moveTimer >= moveInterval)
                {
                    moveTimer -= moveInterval;
                    Step();
                }
            }

            // Face animation runs every frame, even when input locked
            UpdateFaceDecorations();
        }

        private void Step()
        {
            direction = queuedDirection;

            var newHead = cells[0] + direction;

            // Wall collision
            if (newHead.x < 0 || newHead.x >= gridSize.x || newHead.y < 0 || newHead.y >= gridSize.y)
            {
                Die();
                return;
            }

            // Self collision (allow moving into the tail cell since it'll move out)
            for (int i = 0; i < cells.Count - 1; i++)
            {
                if (cells[i] == newHead) { Die(); return; }
            }

            // Check what's at the new head cell
            var pickup = GridArena.Instance != null ? GridArena.Instance.GetPickupAtCell(newHead) : null;
            bool ateApple = false;
            if (pickup != null)
            {
                if (pickup is Apple)
                {
                    ateApple = true;
                    pickup.Consume();
                    pendingGrowth += 1;
                    GameStats.Instance?.EatApple();
                    ParticleBurst.Spawn(CellToWorld(newHead), new Color(1f, 0.4f, 0.4f), count: 12, spread: 3f, lifetime: 0.55f, size: 0.18f);
                }
                else if (pickup is Bomb)
                {
                    pickup.Consume();
                    ParticleBurst.Spawn(CellToWorld(newHead), new Color(1f, 0.6f, 0.1f), count: 24, spread: 6f, lifetime: 0.9f, size: 0.25f);
                    ParticleBurst.Spawn(CellToWorld(newHead), new Color(1f, 1f, 0.4f), count: 16, spread: 4f, lifetime: 0.7f, size: 0.18f);
                    Die();
                    return;
                }
            }

            if (ateApple)
            {
                eatPulseUntil = Time.unscaledTime + EatPulseDuration;
            }

            // Move: shift body
            previousWorldPositions.Clear();
            previousWorldPositions.AddRange(cellWorldPositions);

            cells.Insert(0, newHead);
            if (pendingGrowth > 0)
            {
                pendingGrowth -= 1;
                CreateSegment();
                // Newly added segment starts at the new tail's position so it doesn't sweep across
                int last = segments.Count - 1;
                cellWorldPositions.Add(CellToWorld(cells[last]));
                previousWorldPositions.Add(cellWorldPositions[last]);
            }
            else
            {
                cells.RemoveAt(cells.Count - 1);
            }

            // Recompute world positions
            cellWorldPositions.Clear();
            for (int i = 0; i < cells.Count; i++)
                cellWorldPositions.Add(CellToWorld(cells[i]));

            UpdateSegmentVisuals();
        }

        // Face decorations — children of the snake root (not the head segment, which is scaled)
        private GameObject cachedEyeWhiteL, cachedEyeWhiteR;
        private GameObject cachedPupilL, cachedPupilR;
        private GameObject cachedTongue;
        private float eatPulseUntil;
        private const float EatPulseDuration = 0.20f;

        private void CreateSegment()
        {
            var go = new GameObject($"Segment_{segments.Count}");
            go.transform.SetParent(transform, false);
            float s = cellSize * bodyScaleMul;
            go.transform.localScale = new Vector3(s, s, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = segmentSprite;
            sr.sortingOrder = 10;
            segments.Add(go.transform);
        }

        private void UpdateSegmentVisuals()
        {
            // Uniform: every segment is the same sprite, same scale, same color.
            // Segments overlap substantially so straight runs look like a continuous
            // bar and turns stay visually connected through the diagonal moment.
            // sortingOrder kept low (head=20) so face decorations (24-26) render above.
            float worldScale = cellSize * bodyScaleMul;
            int n = segments.Count;
            for (int i = 0; i < n; i++)
            {
                var sr = segments[i].GetComponent<SpriteRenderer>();
                sr.sprite = segmentSprite;
                sr.color = headColor;
                sr.sortingOrder = 20 - i;
                segments[i].localScale = new Vector3(worldScale, worldScale, 1f);
            }
        }

        private void EnsureFaceDecorations()
        {
            if (cachedEyeWhiteL != null) return;
            var roundSprite = circleSprite != null ? circleSprite : segmentSprite;

            cachedEyeWhiteL = CreateFaceSprite("EyeWhiteL", Color.white, cellSize * 0.42f, 25, roundSprite);
            cachedEyeWhiteR = CreateFaceSprite("EyeWhiteR", Color.white, cellSize * 0.42f, 25, roundSprite);
            cachedPupilL = CreateFaceSprite("PupilL", new Color(0.05f, 0.05f, 0.10f), cellSize * 0.22f, 26, roundSprite);
            cachedPupilR = CreateFaceSprite("PupilR", new Color(0.05f, 0.05f, 0.10f), cellSize * 0.22f, 26, roundSprite);
            var tSprite = tongueSprite != null ? tongueSprite : segmentSprite;
            cachedTongue = CreateFaceSprite("Tongue", new Color(0.95f, 0.30f, 0.50f), 1f, 24, tSprite);
            cachedTongue.transform.localScale = new Vector3(cellSize * 0.30f, cellSize * 0.10f, 1f);
        }

        private GameObject CreateFaceSprite(string name, Color color, float worldSize, int sortingOrder, Sprite sprite)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false); // Parent to snake root, NOT scaled head
            go.transform.localScale = new Vector3(worldSize, worldSize, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = sortingOrder;
            return go;
        }

        private void UpdateFaceDecorations()
        {
            if (segments.Count == 0) return;
            EnsureFaceDecorations();

            Vector3 headPos = segments[0].position;
            Vector3 fwd  = new Vector3(direction.x, direction.y, 0);
            Vector3 side = new Vector3(-direction.y, direction.x, 0);

            // Eye sclera positions — slightly forward, splayed to the sides
            float eyeFwd = cellSize * 0.10f;
            float eyeSide = cellSize * 0.18f;
            Vector3 leftEye = headPos + fwd * eyeFwd + side * eyeSide;
            Vector3 rightEye = headPos + fwd * eyeFwd - side * eyeSide;
            cachedEyeWhiteL.transform.position = leftEye;
            cachedEyeWhiteR.transform.position = rightEye;

            // Pupils track the nearest apple (or look forward if none)
            Apple nearest = FindNearestApple(headPos);
            Vector3 pupilDir = fwd;
            float distToApple = float.MaxValue;
            if (nearest != null)
            {
                Vector3 toApple = nearest.transform.position - headPos;
                distToApple = toApple.magnitude;
                if (distToApple > 0.001f) pupilDir = toApple / distToApple;
            }

            float pupilOffset = cellSize * 0.10f;
            cachedPupilL.transform.position = leftEye + pupilDir * pupilOffset;
            cachedPupilR.transform.position = rightEye + pupilDir * pupilOffset;

            // Tongue: extends forward, wiggles when an apple is nearby
            bool nearApple = nearest != null && distToApple < cellSize * 3.0f;
            float baseTongueLen = cellSize * 0.30f;
            float tongueLen = nearApple
                ? baseTongueLen * (1.6f + Mathf.Sin(Time.unscaledTime * 22f) * 0.30f)
                : baseTongueLen;
            float tongueThick = cellSize * 0.10f;

            Vector3 tongueAnchor = headPos + fwd * (cellSize * 0.32f);
            Vector3 tongueCenter = tongueAnchor + fwd * (tongueLen * 0.5f);
            cachedTongue.transform.position = tongueCenter;
            cachedTongue.transform.localScale = new Vector3(tongueLen, tongueThick, 1f);
            float angle = Mathf.Atan2(fwd.y, fwd.x) * Mathf.Rad2Deg;
            cachedTongue.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Eat pulse — head briefly scales up after eating
            if (Time.unscaledTime < eatPulseUntil)
            {
                float t = 1f - (eatPulseUntil - Time.unscaledTime) / EatPulseDuration; // 0..1
                float pulse = 1f + Mathf.Sin(t * Mathf.PI) * 0.20f;
                float baseScale = cellSize * bodyScaleMul;
                segments[0].localScale = new Vector3(baseScale * pulse, baseScale * pulse, 1f);
            }
        }

        private Apple FindNearestApple(Vector3 from)
        {
#if UNITY_2023_1_OR_NEWER
            var apples = Object.FindObjectsByType<Apple>(FindObjectsSortMode.None);
#else
            var apples = Object.FindObjectsOfType<Apple>();
#endif
            Apple nearest = null;
            float minSqr = float.MaxValue;
            foreach (var a in apples)
            {
                float d = (a.transform.position - from).sqrMagnitude;
                if (d < minSqr) { minSqr = d; nearest = a; }
            }
            return nearest;
        }

        private void Die()
        {
            if (dead) return;
            dead = true;
            CameraScreenFlash.Instance?.Flash(new Color(1f, 0.2f, 0.2f, 0.7f), 0.3f);
            GameStats.Instance?.Die();
        }
    }
}
