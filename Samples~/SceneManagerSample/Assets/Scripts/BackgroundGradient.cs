using UnityEngine;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Stretches a sprite to fill the camera and (optionally) shifts hue over time.
    /// Used for cheap themed backgrounds without textures.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundGradient : MonoBehaviour
    {
        [SerializeField] private Color colorA = Color.black;
        [SerializeField] private Color colorB = Color.black;
        [SerializeField] private float cycleSpeed = 0f;

        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            sr.color = colorA;
            sr.sortingOrder = -100;
        }

        private void Update()
        {
            if (cycleSpeed <= 0f) return;
            float t = (Mathf.Sin(Time.unscaledTime * cycleSpeed) + 1f) * 0.5f;
            sr.color = Color.Lerp(colorA, colorB, t);
        }

        public void SetColors(Color a, Color b) { colorA = a; colorB = b; if (sr != null) sr.color = a; }
    }
}
