using UnityEngine;
using UnityEngine.UI;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Pause overlay controller. Sets Time.timeScale = 0 while active so all gameplay
    /// freezes — proves WaitForSecondsRealtime in SceneManager works under timeScale = 0.
    /// Buttons: Resume (Pop), Stats (Push), Restart Level (Reload), Quit to Title (LoadScene).
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button statsButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private string statsOverlay = "StatsOverlay";
        [SerializeField] private string titleScene = "TitleScreen";
        [SerializeField] private SceneTransition_UMFOSS instantTransition;
        [SerializeField] private SceneTransition_UMFOSS fadeTransition;

        private float previousTimeScale = 1f;

        private void OnEnable()
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
        }

        private void Start()
        {
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResume);
            if (statsButton != null) statsButton.onClick.AddListener(OnStats);
            if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
            if (titleButton != null) titleButton.onClick.AddListener(OnTitle);
        }

        private void OnResume() => SceneManager_UMFOSS.Instance?.Pop();
        private void OnStats() => SceneManager_UMFOSS.Instance?.Push(statsOverlay, instantTransition);
        private void OnRestart() => SceneManager_UMFOSS.Instance?.ReloadScene(fadeTransition);
        private void OnTitle() => SceneManager_UMFOSS.Instance?.LoadScene(titleScene, fadeTransition);
    }
}
