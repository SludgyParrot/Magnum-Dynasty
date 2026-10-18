using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "Story Graph", menuName = "Parrot Code/Story Graph")]
public sealed class StoryGraph : NodeGraph 
{
    [SerializeField]
    public List<StoryGraphNode> GraphNodes = new List<StoryGraphNode>();

    public EntryNode Entry {  get; private set; }

    public bool Completed { get; private set; }

    public bool HasPrerequisiteGraphs
    {
        get
        {
             if(Entry.PrerequisiteGraphs == null || 
                Entry.PrerequisiteGraphs.Count == 0) return false;
            return Entry.PrerequisiteGraphs.Count(x => x.Completed == false) > 0;
        }
    }

    public void Initialize()
    {
        if (nodes.Count == 0) return;

        foreach (Node node in nodes)
        {
            if (node is EntryNode entry)
                Entry = entry;

            StoryGraphNode graphNode = node as StoryGraphNode;

            if(!GraphNodes.Contains(graphNode))
                GraphNodes.Add(graphNode);
        }
    }

    public void CompleteGraph()
        => Completed = true;

    public void ResetGraph()
        => Completed = false;
}