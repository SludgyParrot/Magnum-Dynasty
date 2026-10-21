using System.Collections.Generic;
using UnityEngine;

public sealed class TriggerEventsNode : DefaultStoryNode
{
    [field: SerializeField, Space(5)]
    public List<EventConfig> Events { get; private set; }
}
