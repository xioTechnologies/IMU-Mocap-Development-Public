using UnityEngine;
using ActionQueue = StaticActionQueue<GlobalObjectActivator, bool>;

public sealed class GlobalObjectActivator : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterQueueProcessing() => ActionQueue.RegisterOnIngestData((obj, active) => obj.gameObject.SetActive(active));

    [SerializeField] private GlobalSetting setting;

    private void Awake()
    {
        setting.OnValueChanged += OnValueChanged;

        OnValueChanged(setting.Value);
    }
    
    private void OnValueChanged(bool obj) => ActionQueue.Enqueue(this, obj); 

    private void OnDestroy() => setting.OnValueChanged -= OnValueChanged;
}