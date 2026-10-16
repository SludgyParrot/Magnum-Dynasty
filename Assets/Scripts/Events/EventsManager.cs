using UnityEngine;
using UnityEngine.Events;

public sealed class EventsManager : SingletonInstance<EventsManager>
{
    [SerializeField]
    public UnityEvent<float, float> OnSpeedOMeterValueChanged = 
        new UnityEvent<float, float>();

    public UnityEvent<StoryEntryEvent> OnStoryEntryEvent =
        new UnityEvent<StoryEntryEvent>();

    public UnityEvent<ScreenTextConfig> OnDisplayTooltipEvent = 
        new UnityEvent<ScreenTextConfig>();

    public UnityEvent<EventConfig> OnTriggeredEvent =
     new UnityEvent<EventConfig>();

    public EventConfig CurrentTriggeredEventConfig {  get; private set; }

    public bool IsCurrentEvent(EventConfig config)
        => config.GetType() == CurrentTriggeredEventConfig.GetType();

    private void OnEnable()
        => OnTriggeredEvent.AddListener(SetCurrentTriggeredEventConfig);

    private void OnDisable()
        => OnTriggeredEvent.RemoveListener(SetCurrentTriggeredEventConfig);

    private void SetCurrentTriggeredEventConfig(EventConfig config)
        => CurrentTriggeredEventConfig = config;

    public void OnSceneExit()
    {
        OnSpeedOMeterValueChanged?.RemoveAllListeners();
        OnStoryEntryEvent?.RemoveAllListeners();
        OnDisplayTooltipEvent?.RemoveAllListeners();
    }
}
