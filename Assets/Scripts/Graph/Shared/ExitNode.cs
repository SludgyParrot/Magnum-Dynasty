using UnityEngine;

public sealed class ExitNode : StoryGraphNode
{
    [field: SerializeField]
    public StoryGraph ExitGraph {  get; private set; }

    [Input]
    public int input;
}
