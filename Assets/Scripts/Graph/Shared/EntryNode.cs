using System.Collections.Generic;
using UnityEngine;

public sealed class EntryNode : StoryGraphNode
{
    [field: SerializeField]
    public StoryEntryEvent EntryEvent { get; private set; }

    [field: SerializeField, Space(5)]
    public List<StoryGraph> PrerequisiteGraphs { get; private set; }

    [Output]
    public int output;
}
