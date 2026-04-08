using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameplayMechanicsUMFOSS.Core;
using GameplayMechanicsUMFOSS.Systems;

namespace GameplayMechanicsUMFOSS.Samples.SceneManagerSample
{
    /// <summary>
    /// Subscribes to ALL 7 SceneManager events and displays the most recent few
    /// in a scrolling text panel. This is the visible proof that every event fires.
    /// </summary>
    public class EventLogPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int maxLines = 5;

        private readonly Queue<string> lines = new Queue<string>();

        private void OnEnable()
        {
            EventBus.Subscribe<SceneLoadStartEvent>(OnLoadStart);
            EventBus.Subscribe<SceneLoadProgressEvent>(OnLoadProgress);
            EventBus.Subscribe<SceneLoadCompleteEvent>(OnLoadComplete);
            EventBus.Subscribe<ScenePushedEvent>(OnPushed);
            EventBus.Subscribe<ScenePoppedEvent>(OnPopped);
            EventBus.Subscribe<SceneReloadedEvent>(OnReloaded);
            EventBus.Subscribe<InputLockEvent>(OnInputLock);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoadStartEvent>(OnLoadStart);
            EventBus.Unsubscribe<SceneLoadProgressEvent>(OnLoadProgress);
            EventBus.Unsubscribe<SceneLoadCompleteEvent>(OnLoadComplete);
            EventBus.Unsubscribe<ScenePushedEvent>(OnPushed);
            EventBus.Unsubscribe<ScenePoppedEvent>(OnPopped);
            EventBus.Unsubscribe<SceneReloadedEvent>(OnReloaded);
            EventBus.Unsubscribe<InputLockEvent>(OnInputLock);
        }

        private void Push(string color, string label, string detail)
        {
            lines.Enqueue($"<color=#{color}>● {label}</color> <size=20>{detail}</size>");
            while (lines.Count > maxLines) lines.Dequeue();
            if (text != null) text.text = string.Join("\n", lines);
        }

        private void OnLoadStart(SceneLoadStartEvent e)         => Push("4FC3F7", "LoadStart",    $"{e.fromScene} → {e.toScene}");
        private float lastProgress;
        private void OnLoadProgress(SceneLoadProgressEvent e)
        {
            // Throttle progress to avoid spam — only log at 25%, 50%, 75%, 100%
            float p = Mathf.Round(e.progress * 4f) / 4f;
            if (Mathf.Approximately(p, lastProgress)) return;
            lastProgress = p;
            Push("81D4FA", "LoadProgress", $"{(int)(p * 100)}%");
        }
        private void OnLoadComplete(SceneLoadCompleteEvent e)   { lastProgress = -1; Push("69F0AE", "LoadComplete", e.sceneName); }
        private void OnPushed(ScenePushedEvent e)               => Push("FFD54F", "Pushed",       e.sceneName);
        private void OnPopped(ScenePoppedEvent e)               => Push("FFB74D", "Popped",       e.sceneName);
        private void OnReloaded(SceneReloadedEvent e)           => Push("CE93D8", "Reloaded",     e.sceneName);
        private void OnInputLock(InputLockEvent e)              => Push("B0BEC5", "InputLock",    e.locked ? "locked" : "unlocked");
    }
}
