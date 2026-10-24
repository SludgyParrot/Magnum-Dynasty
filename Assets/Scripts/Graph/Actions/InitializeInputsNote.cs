using System.Collections.Generic;
using UnityEngine;

public sealed class InitializeInputsNote : DefaultStoryNode
{
    [field: SerializeField, Space(5)]
    public List<EventConfig> InputActions { get; private set; }
}
