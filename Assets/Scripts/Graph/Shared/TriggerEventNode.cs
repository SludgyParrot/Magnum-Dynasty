using UnityEngine;

public sealed class TriggerEventNode : DefaultStoryNode
{
    [field: SerializeField, Space(5)]
    public EventConfig Event { get; private set; }
}
