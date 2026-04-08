using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Spawns apples and (optionally) bombs on random empty grid cells.
    /// Apples respawn one-for-one when eaten. Bombs have a per-bomb lifetime —
    /// when expired they vanish (with a poof) and a new one spawns in a fresh cell.
    /// </summary>
    public class FoodSpawner : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private int desiredApples = 1;
        [SerializeField] private int desiredBombs = 0;
        [SerializeField] private float spawnInterval = 0.5f;

        [Header("Bomb relocation")]
        [SerializeField] private float bombLifetimeMin = 2.0f;
        [SerializeField] private float bombLifetimeMax = 3.0f;

        [Header("Prefab data — set at runtime")]
        public System.Func<GameObject> appleFactory;
        public System.Func<GameObject> bombFactory;

        private float timer;
        private Snake snake;

        private void Start()
        {
            snake = FindFirstObjectByType<Snake>();
            for (int i = 0; i < desiredApples; i++) SpawnApple();
            for (int i = 0; i < desiredBombs; i++) SpawnBomb();
        }

        public void SetTargets(int apples, int bombs) { desiredApples = apples; desiredBombs = bombs; }

        private void Update()
        {
            // Expire old bombs (they vanish + a fresh one spawns in a different cell)
            var bombs = FindObjectsByType<Bomb>(FindObjectsSortMode.None);
            foreach (var bomb in bombs)
            {
                if (Time.time - bomb.SpawnedAt >= bomb.Lifetime)
                {
                    ParticleBurst.Spawn(bomb.transform.position, new Color(1f, 0.5f, 0.1f), count: 10, spread: 2.5f, lifetime: 0.45f, size: 0.18f);
                    bomb.Consume();
                }
            }

            timer += Time.deltaTime;
            if (timer < spawnInterval) return;
            timer = 0f;

            int currentApples = 0;
            foreach (var _ in FindObjectsByType<Apple>(FindObjectsSortMode.None)) currentApples++;
            int currentBombs = 0;
            foreach (var _ in FindObjectsByType<Bomb>(FindObjectsSortMode.None)) currentBombs++;

            if (currentApples < desiredApples) SpawnApple();
            if (currentBombs < desiredBombs) SpawnBomb();
        }

        private void SpawnApple()
        {
            if (appleFactory == null || GridArena.Instance == null) return;
            var cell = FindRandomEmptyCell();
            if (cell == null) return;
            var go = appleFactory();
            go.transform.position = GridArena.Instance.CellToWorld(cell.Value);
            var apple = go.GetComponent<Apple>();
            apple.Cell = cell.Value;
            apple.SpawnedAt = Time.time;
            GridArena.Instance.RegisterPickup(cell.Value, apple);
        }

        private void SpawnBomb()
        {
            if (bombFactory == null || GridArena.Instance == null) return;
            var cell = FindRandomEmptyCell();
            if (cell == null) return;
            var go = bombFactory();
            go.transform.position = GridArena.Instance.CellToWorld(cell.Value);
            var bomb = go.GetComponent<Bomb>();
            bomb.Cell = cell.Value;
            bomb.SpawnedAt = Time.time;
            bomb.Lifetime = Random.Range(bombLifetimeMin, bombLifetimeMax);
            GridArena.Instance.RegisterPickup(cell.Value, bomb);
        }

        private Vector2Int? FindRandomEmptyCell()
        {
            var arena = GridArena.Instance;
            if (arena == null) return null;
            for (int attempt = 0; attempt < 80; attempt++)
            {
                var cell = new Vector2Int(Random.Range(0, arena.GridSize.x), Random.Range(0, arena.GridSize.y));
                if (arena.IsCellEmpty(cell, snake)) return cell;
            }
            return null;
        }
    }
}
