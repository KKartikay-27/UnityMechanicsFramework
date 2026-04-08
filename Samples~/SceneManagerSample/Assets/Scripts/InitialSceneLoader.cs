using UnityEngine;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Lives in the persistent scene. On Start(), tells the SceneManager to load
    /// the title screen. Without this, PersistentScene sits empty forever.
    /// </summary>
    public class InitialSceneLoader : MonoBehaviour
    {
        [SerializeField] private string initialScene = "TitleScreen";
        [SerializeField] private SceneTransition_UMFOSS transition;

        private void Start()
        {
            if (SceneManager_UMFOSS.Instance == null)
            {
                Debug.LogError("[InitialSceneLoader] No SceneManager_UMFOSS instance found.");
                return;
            }
            SceneManager_UMFOSS.Instance.LoadScene(initialScene, transition);
        }
    }
}
