using UnityEngine;
using UnityEngine.Events;

public sealed class EventsManager : SingletonInstance<EventsManager>
{
    [SerializeField]
    public UnityEvent<float, float> OnSpeedOMeterValueChanged = 
        new UnityEvent<float, float>();

    public UnityEvent<StoryEntryEvent> OnStoryEntryEvent =
        new UnityEvent<StoryEntryEvent>();

    public void OnSceneExit()
    {
        OnSpeedOMeterValueChanged?.RemoveAllListeners();
        OnStoryEntryEvent?.RemoveAllListeners();
    }
}
