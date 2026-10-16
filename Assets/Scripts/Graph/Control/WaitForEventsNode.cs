using System.Collections.Generic;
using UnityEngine;

public sealed class WaitForEventsNode : DefaultStoryNode
{
    [field: SerializeField]
    public List<EventConfig> Events { get; private set; }
}
