using UnityEngine;

public sealed class StoryGraphEntryEventHandler : TriggerEvent
{
    [SerializeField, Space(5)]
    private EventConfig entryEvent;

    public override void Trigger()
        => EventsManager.Instance.OnTriggeredEvent?.Invoke(entryEvent);
}
