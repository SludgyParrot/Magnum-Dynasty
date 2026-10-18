using UnityEngine;
using UnityEngine.Events;

public sealed class EventsManager : SingletonInstance<EventsManager>
{
    [SerializeField]
    public UnityEvent<float, float> OnSpeedOMeterValueChanged = 
        new UnityEvent<float, float>();

    public UnityEvent<ScreenTextConfig> OnDisplayTooltipEvent = 
        new UnityEvent<ScreenTextConfig>();

    public UnityEvent<EventConfig> OnTriggeredEvent =
        new UnityEvent<EventConfig>();

    public UnityEvent<ScreenConfig> OnScreenConfigurationEvent =
        new UnityEvent<ScreenConfig>();

    public EventConfig CurrentTriggeredEventConfig {  get; private set; }

    public bool IsCurrentEvent(EventConfig config)
    {
        if (CurrentTriggeredEventConfig == null) return false;
        return config.GetType() == CurrentTriggeredEventConfig.GetType();
    }

    private void OnEnable()
        => OnTriggeredEvent.AddListener(SetCurrentTriggeredEventConfig);

    private void OnDisable()
        => OnTriggeredEvent.RemoveListener(SetCurrentTriggeredEventConfig);

    private void SetCurrentTriggeredEventConfig(EventConfig config)
        => CurrentTriggeredEventConfig = config;

    public void OnSceneExit()
    {
        OnSpeedOMeterValueChanged?.RemoveAllListeners();
        OnTriggeredEvent?.RemoveAllListeners();
        OnDisplayTooltipEvent?.RemoveAllListeners();
    }
}
