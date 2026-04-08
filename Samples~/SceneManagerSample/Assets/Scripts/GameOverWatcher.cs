using UnityEngine;
using GameplayMechanicsUMFOSS.Core;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Lives in the persistent scene. Listens for SnakeDiedEvent and triggers the
    /// game over scene load with a short delay so the death particles can play out.
    /// </summary>
    public class GameOverWatcher : MonoBehaviour
    {
        [SerializeField] private string gameOverScene = "GameOver";
        [SerializeField] private SceneTransition_UMFOSS transition;
        [SerializeField] private float delay = 0.6f;

        private void OnEnable()  => EventBus.Subscribe<SnakeDiedEvent>(OnDied);
        private void OnDisable() => EventBus.Unsubscribe<SnakeDiedEvent>(OnDied);

        private void OnDied(SnakeDiedEvent _)
        {
            if (SceneManager_UMFOSS.Instance == null) return;
            if (SceneManager_UMFOSS.Instance.IsTransitioning()) return;
            Invoke(nameof(GoToGameOver), delay);
        }

        private void GoToGameOver()
        {
            if (SceneManager_UMFOSS.Instance == null) return;
            if (SceneManager_UMFOSS.Instance.IsTransitioning()) return;
            SceneManager_UMFOSS.Instance.LoadScene(gameOverScene, transition);
        }
    }
}
