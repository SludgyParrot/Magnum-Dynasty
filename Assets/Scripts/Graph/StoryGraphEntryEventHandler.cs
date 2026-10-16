using UnityEngine;

public sealed class StoryGraphEntryEventHandler : TriggerEvent
{
    [SerializeField, Space(5)]
    private StoryEntryEvent entryEvent;

    public override void Trigger()
        => EventsManager.Instance.OnStoryEntryEvent?.Invoke(entryEvent);
}
