using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Spawns N small colored squares that fly outward and fade. Pure script — no ParticleSystem.
    /// Call ParticleBurst.Spawn(position, color) from anywhere.
    /// </summary>
    public class ParticleBurst : MonoBehaviour
    {
        private static Sprite cachedSprite;

        public static void Spawn(Vector3 position, Color color, int count = 8, float spread = 2f, float lifetime = 0.5f, float size = 0.2f)
        {
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("BurstParticle");
                go.transform.position = position;
                go.transform.localScale = new Vector3(size, size, 1f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = GetSprite();
                sr.color = color;
                sr.sortingOrder = 50;

                var p = go.AddComponent<ParticleBurst>();
                float angle = (i / (float)count) * Mathf.PI * 2f;
                p.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spread;
                p.lifetime = lifetime;
                p.startColor = color;
            }
        }

        private static Sprite GetSprite()
        {
            if (cachedSprite != null) return cachedSprite;
            // Try to find the white square asset created by the editor generator.
            cachedSprite = Resources.Load<Sprite>("WhiteSquare");
            if (cachedSprite != null) return cachedSprite;

#if UNITY_EDITOR
            cachedSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/SceneManagerDemo/WhiteSquare.png");
            if (cachedSprite != null) return cachedSprite;
#endif

            // Last resort: build a runtime sprite (won't survive scene save but works in play mode)
            var tex = Texture2D.whiteTexture;
            cachedSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
            return cachedSprite;
        }

        private Vector2 velocity;
        private float lifetime;
        private float age;
        private Color startColor;
        private SpriteRenderer sr;

        private void Awake() => sr = GetComponent<SpriteRenderer>();

        private void Update()
        {
            age += Time.deltaTime;
            transform.position += (Vector3)(velocity * Time.deltaTime);
            float t = age / lifetime;
            if (sr != null)
            {
                var c = startColor; c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }
            transform.localScale *= 1f - (Time.deltaTime * 0.8f);
            if (age >= lifetime) Destroy(gameObject);
        }
    }
}
