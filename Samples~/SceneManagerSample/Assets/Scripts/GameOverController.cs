using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Game over screen. Retry reloads the current level.
    /// Title returns to the main menu.
    /// </summary>
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private string titleScene = "TitleScreen";
        [SerializeField] private SceneTransition_UMFOSS transition;

        private void Start()
        {
            if (retryButton != null) retryButton.onClick.AddListener(OnRetry);
            if (titleButton != null) titleButton.onClick.AddListener(OnTitle);

            var stats = GameStats.Instance;
            if (stats != null && statsText != null)
            {
                statsText.text = $"Final score: {stats.CurrentLevelScore}\nTotal apples: {stats.TotalApplesEaten}\nLevels cleared: {stats.LevelsCleared.Count} / 3";
            }
        }

        private void OnRetry()
        {
            var level = GameStats.Instance != null ? GameStats.Instance.CurrentLevelName : "Level_Garden";
            if (string.IsNullOrEmpty(level)) level = "Level_Garden";
            SceneManager_UMFOSS.Instance?.LoadScene(level, transition);
        }

        private void OnTitle()
        {
            SceneManager_UMFOSS.Instance?.LoadScene(titleScene, transition);
        }
    }
}
