using System.Collections.Generic;
using UnityEngine;

public sealed class EntryNode : StoryGraphNode
{
    [field: SerializeField]
    public List<EventConfig> EntryEvents { get; private set; } =
        new List<EventConfig>();

    [field: SerializeField, Space(5)]
    public List<StoryGraph> PrerequisiteGraphs { get; private set; } = 
        new List<StoryGraph>();

    [Output]
    public int output;
}
