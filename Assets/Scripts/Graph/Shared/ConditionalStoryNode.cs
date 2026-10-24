using UnityEngine;

public abstract class ConditionalStoryNode : StoryGraphNode
{
    [Input]
    public int input;

    [Output]
    public int _if;

    [Output]
    public int _else;

    public string PrimaryOutputPortName { get; private set; } = "_if";
    public string SecondaryOutputPortName { get; private set; } = "_else";
}
