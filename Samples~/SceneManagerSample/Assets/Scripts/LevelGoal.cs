using UnityEngine;
using GameplayMechanicsUMFOSS.Core;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Per-level component. Holds level identity + advancement target. Subscribes to
    /// LevelClearedEvent and triggers a LoadScene to the next level (or Victory).
    /// </summary>
    public class LevelGoal : MonoBehaviour
    {
        [SerializeField] private string levelName;
        [SerializeField] private int appleTarget = 5;
        [SerializeField] private string nextScene;
        [SerializeField] private SceneTransition_UMFOSS transition;

        public string LevelName => levelName;
        public int AppleTarget => appleTarget;

        private bool advancing;

        public void Configure(string name, int target, string next)
        {
            levelName = name;
            appleTarget = target;
            nextScene = next;
        }

        private void Start()
        {
            GameStats.Instance?.EnterLevel(levelName, appleTarget);
        }

        private void OnEnable() => EventBus.Subscribe<LevelClearedEvent>(OnLevelCleared);
        private void OnDisable() => EventBus.Unsubscribe<LevelClearedEvent>(OnLevelCleared);

        private void OnLevelCleared(LevelClearedEvent e)
        {
            if (advancing) return;
            if (e.levelName != levelName) return;
            advancing = true;
            GameStats.Instance?.MarkLevelCleared(levelName);
            if (SceneManager_UMFOSS.Instance == null) return;
            SceneManager_UMFOSS.Instance.LoadScene(nextScene, transition);
        }
    }
}
