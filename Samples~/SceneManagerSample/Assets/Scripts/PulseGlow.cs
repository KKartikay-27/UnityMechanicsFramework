using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Adds a sin-wave pulse to a SpriteRenderer's alpha and scale.
    /// Used for glow halos behind important objects.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PulseGlow : MonoBehaviour
    {
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float minAlpha = 0.25f;
        [SerializeField] private float maxAlpha = 0.7f;
        [SerializeField] private float minScale = 0.9f;
        [SerializeField] private float maxScale = 1.15f;

        private SpriteRenderer sr;
        private Vector3 baseScale;
        private Color baseColor;
        private float seed;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
            baseColor = sr.color;
            seed = Random.value * 10f;
        }

        private void Update()
        {
            float t = (Mathf.Sin((Time.unscaledTime + seed) * speed) + 1f) * 0.5f;
            float a = Mathf.Lerp(minAlpha, maxAlpha, t);
            float s = Mathf.Lerp(minScale, maxScale, t);
            var c = baseColor; c.a = a;
            sr.color = c;
            transform.localScale = baseScale * s;
        }
    }
}
