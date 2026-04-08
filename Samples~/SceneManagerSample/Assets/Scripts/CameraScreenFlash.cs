using UnityEngine;
using UnityEngine.UI;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Screen flash overlay that lives in the persistent scene. Fades a full-screen
    /// colored UI Image alpha down over a configurable duration.
    /// </summary>
    public class CameraScreenFlash : MonoBehaviour
    {
        public static CameraScreenFlash Instance { get; private set; }

        [SerializeField] private Image flashImage;

        private float flashUntil;
        private Color flashColor = Color.white;
        private float duration = 0.3f;

        private void Awake()
        {
            Instance = this;
        }

        public void Flash(Color color, float dur = 0.3f)
        {
            flashColor = color;
            duration = Mathf.Max(0.05f, dur);
            flashUntil = Time.unscaledTime + duration;
            if (flashImage != null) flashImage.color = color;
        }

        private void Update()
        {
            if (flashImage == null) return;
            if (Time.unscaledTime >= flashUntil)
            {
                if (flashImage.color.a > 0f)
                {
                    var c = flashImage.color; c.a = 0f; flashImage.color = c;
                }
                return;
            }
            float remaining = flashUntil - Time.unscaledTime;
            float a = Mathf.Clamp01(remaining / duration);
            var col = flashColor; col.a = a * flashColor.a;
            flashImage.color = col;
        }
    }
}
