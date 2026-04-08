using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>Base for things the snake can collide with on a cell (apple, bomb).</summary>
    public abstract class Pickup : MonoBehaviour
    {
        public Vector2Int Cell { get; set; }
        public float SpawnedAt { get; set; }

        public virtual void Consume()
        {
            if (GridArena.Instance != null)
                GridArena.Instance.UnregisterPickup(Cell);
            Destroy(gameObject);
        }
    }

    public class Apple : Pickup { }

    public class Bomb : Pickup
    {
        public float Lifetime { get; set; } = 3f;
    }
}
