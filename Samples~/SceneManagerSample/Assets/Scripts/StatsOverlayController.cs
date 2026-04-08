using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Stats overlay pushed on top of the pause menu. Demonstrates push-on-push.
    /// </summary>
    public class StatsOverlayController : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI applesText;
        [SerializeField] private TextMeshProUGUI levelsText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private SceneTransition_UMFOSS instantTransition;

        private void Start()
        {
            if (closeButton != null) closeButton.onClick.AddListener(OnClose);

            var stats = GameStats.Instance;
            if (stats != null)
            {
                if (applesText != null) applesText.text = $"Total Apples Eaten: {stats.TotalApplesEaten}";
                if (levelsText != null) levelsText.text = $"Levels Cleared: {stats.LevelsCleared.Count} / 3";
                if (scoreText != null) scoreText.text = $"Current Run: {stats.CurrentLevelScore} / {stats.CurrentLevelTarget}";
            }
        }

        private void OnClose()
        {
            SceneManager_UMFOSS.Instance?.Pop(instantTransition);
        }
    }
}
