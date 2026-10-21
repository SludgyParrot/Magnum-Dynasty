using System.Collections.Generic;
using UnityEngine;

public sealed class ScreenEventsNode : DefaultStoryNode
{
    [field: SerializeField]
    public List<ScreenUIEventConfig> Events { get; private set; }
}
