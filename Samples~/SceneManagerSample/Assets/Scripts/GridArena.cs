using System.Collections.Generic;
using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Per-scene singleton that holds the play grid bounds and a registry of pickups
    /// (apples + bombs) by cell. Snake queries this to know what's at the next cell.
    /// </summary>
    public class GridArena : MonoBehaviour
    {
        public static GridArena Instance { get; private set; }

        [SerializeField] private Vector2Int gridSize = new Vector2Int(32, 18);
        [SerializeField] private float cellSize = 0.4f;

        private readonly Dictionary<Vector2Int, Pickup> pickupsByCell = new Dictionary<Vector2Int, Pickup>();

        public Vector2Int GridSize => gridSize;
        public float CellSize => cellSize;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void RegisterPickup(Vector2Int cell, Pickup pickup)
        {
            pickupsByCell[cell] = pickup;
        }

        public void UnregisterPickup(Vector2Int cell)
        {
            pickupsByCell.Remove(cell);
        }

        public Pickup GetPickupAtCell(Vector2Int cell)
        {
            pickupsByCell.TryGetValue(cell, out var p);
            return p;
        }

        public bool IsCellEmpty(Vector2Int cell, Snake snake)
        {
            if (pickupsByCell.ContainsKey(cell)) return false;
            if (snake != null && snake.OccupiesCell(cell)) return false;
            return true;
        }

        public Vector3 CellToWorld(Vector2Int c)
        {
            float x = (c.x - gridSize.x * 0.5f + 0.5f) * cellSize;
            float y = (c.y - gridSize.y * 0.5f + 0.5f) * cellSize;
            return new Vector3(x, y, 0f);
        }
    }
}
