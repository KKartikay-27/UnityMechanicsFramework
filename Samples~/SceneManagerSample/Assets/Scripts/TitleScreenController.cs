using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Title screen. Pulses the title text, wires Start/Settings/Quit.
    /// Start triggers a loading-screen transition into Level 1.
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private string nextScene = "Level_Garden";
        [SerializeField] private string settingsOverlay = "SettingsOverlay";
        [SerializeField] private SceneTransition_UMFOSS loadingTransition;
        [SerializeField] private SceneTransition_UMFOSS instantTransition;

        private void Start()
        {
            // Reset stats whenever we hit the title — fresh run every time
            GameStats.Instance?.ResetEverything();

            if (startButton != null) startButton.onClick.AddListener(OnStart);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettings);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuit);
        }

        private void Update()
        {
            if (titleText != null)
            {
                float t = (Mathf.Sin(Time.unscaledTime * 1.5f) + 1f) * 0.5f;
                titleText.color = Color.Lerp(new Color(0.5f, 1f, 0.55f), new Color(0.4f, 0.9f, 1f), t);
                float s = 1f + Mathf.Sin(Time.unscaledTime * 2.2f) * 0.025f;
                titleText.transform.localScale = new Vector3(s, s, 1f);
            }
            if (subtitleText != null)
            {
                float a = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) * 0.5f;
                var c = subtitleText.color; c.a = Mathf.Lerp(0.3f, 1f, a);
                subtitleText.color = c;
            }
        }

        private void OnStart() => SceneManager_UMFOSS.Instance?.LoadScene(nextScene, loadingTransition);
        private void OnSettings() => SceneManager_UMFOSS.Instance?.Push(settingsOverlay, instantTransition);

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
