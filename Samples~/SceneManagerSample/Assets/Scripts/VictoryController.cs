using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Victory screen. Shows total apples + levels cleared and a button back to title.
    /// </summary>
    public class VictoryController : MonoBehaviour
    {
        [SerializeField] private Button titleButton;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private string titleScene = "TitleScreen";
        [SerializeField] private SceneTransition_UMFOSS transition;

        private void Start()
        {
            if (titleButton != null) titleButton.onClick.AddListener(OnTitle);

            var stats = GameStats.Instance;
            if (stats != null && statsText != null)
            {
                statsText.text = $"Total apples eaten: {stats.TotalApplesEaten}\nAll 3 levels cleared!";
            }
        }

        private void OnTitle() => SceneManager_UMFOSS.Instance?.LoadScene(titleScene, transition);
    }
}
