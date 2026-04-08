using UnityEngine;
using TMPro;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Minimal gameplay HUD. Visible only during Level_* scenes.
    /// Three text rows: top-left LEVEL · top-center SCORE · top-right pause hint.
    /// Bottom-left a single tiny stats line. No card backgrounds, no clutter.
    /// </summary>
    public class SnakeHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreLabel;     // top-center
        [SerializeField] private TextMeshProUGUI levelLabel;     // top-left
        [SerializeField] private TextMeshProUGUI pauseHintLabel; // top-right
        [SerializeField] private TextMeshProUGUI statsLine;      // bottom-left
        [SerializeField] private GameObject root;

        private float counter;

        private void Update()
        {
            counter += Time.unscaledDeltaTime;

            var stats = GameStats.Instance;
            var sm = SceneManager_UMFOSS.Instance;

            bool inGameplay = sm != null && IsLevelScene(sm.GetCurrentScene());
            if (root != null && root.activeSelf != inGameplay) root.SetActive(inGameplay);
            if (!inGameplay) return;

            if (scoreLabel != null && stats != null)
                scoreLabel.text = $"<color=#FFFFFF>{stats.CurrentLevelScore}</color> <color=#7FE59E>/ {stats.CurrentLevelTarget}</color>";

            if (levelLabel != null)
                levelLabel.text = $"<color=#9CE5FF>LEVEL</color>  {Pretty(sm.GetCurrentScene())}";

            if (pauseHintLabel != null)
                pauseHintLabel.text = "<color=#FFFFFF80>[ESC] pause</color>";

            if (statsLine != null && stats != null)
                statsLine.text = $"<color=#FFFFFF60>total apples</color> {stats.TotalApplesEaten}   <color=#FFFFFF60>·   stack</color> {sm.GetStackDepth()}   <color=#FFFFFF60>·   persistent</color> {counter:F1}s";
        }

        private static bool IsLevelScene(string name)
        {
            return name == "Level_Garden" || name == "Level_Cyber" || name == "Level_Void";
        }

        private static string Pretty(string scene)
        {
            if (string.IsNullOrEmpty(scene)) return "-";
            return scene.Replace("Level_", "").ToUpper();
        }
    }
}
