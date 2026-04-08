using System.Collections.Generic;
using UnityEngine;
using GameplayMechanicsUMFOSS.Core;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Persistent game stats singleton. Lives in the persistent scene so total apples,
    /// levels cleared, and the current run's score survive every scene transition.
    /// </summary>
    public class GameStats : MonoBehaviour
    {
        public static GameStats Instance { get; private set; }

        // Lifetime totals (never reset on retry)
        public int TotalApplesEaten { get; private set; }

        // Current run state
        public int CurrentLevelScore { get; private set; }
        public int CurrentLevelTarget { get; set; }
        public string CurrentLevelName { get; set; } = "";
        public HashSet<string> LevelsCleared { get; } = new HashSet<string>();
        public bool IsAlive { get; set; } = true;

        public System.Action OnStatsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResetEverything();
        }

        public void ResetEverything()
        {
            TotalApplesEaten = 0;
            CurrentLevelScore = 0;
            CurrentLevelTarget = 0;
            LevelsCleared.Clear();
            CurrentLevelName = "";
            IsAlive = true;
            OnStatsChanged?.Invoke();
        }

        public void EnterLevel(string levelName, int target)
        {
            CurrentLevelName = levelName;
            CurrentLevelTarget = target;
            CurrentLevelScore = 0;
            IsAlive = true;
            OnStatsChanged?.Invoke();
        }

        public void EatApple()
        {
            CurrentLevelScore += 1;
            TotalApplesEaten += 1;
            OnStatsChanged?.Invoke();
            EventBus.Publish(new AppleEatenEvent { score = CurrentLevelScore, target = CurrentLevelTarget });
            if (CurrentLevelScore >= CurrentLevelTarget)
                EventBus.Publish(new LevelClearedEvent { levelName = CurrentLevelName });
        }

        public void Die()
        {
            if (!IsAlive) return;
            IsAlive = false;
            OnStatsChanged?.Invoke();
            EventBus.Publish(new SnakeDiedEvent());
        }

        public void MarkLevelCleared(string levelName)
        {
            LevelsCleared.Add(levelName);
            OnStatsChanged?.Invoke();
        }
    }

    public struct AppleEatenEvent { public int score; public int target; }
    public struct SnakeDiedEvent { }
    public struct LevelClearedEvent { public string levelName; }
}
