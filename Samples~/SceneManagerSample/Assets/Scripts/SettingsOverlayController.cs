using UnityEngine;
using UnityEngine.UI;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Stub settings overlay pushed from the title screen. Proves Push works from
    /// any scene, not just gameplay. Just has a Close button.
    /// </summary>
    public class SettingsOverlayController : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private SceneTransition_UMFOSS instantTransition;

        private void Start()
        {
            if (closeButton != null) closeButton.onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            SceneManager_UMFOSS.Instance?.Pop(instantTransition);
        }
    }
}
