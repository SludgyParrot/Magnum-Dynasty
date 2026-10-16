using XNode;

public abstract class ActionNode : StoryGraphNode
{
    [Input]
    public int input;

    [Output]
    public int output;
}
