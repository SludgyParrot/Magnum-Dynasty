using UnityEngine;

public sealed class ScreenStateNode : DefaultStoryNode
{
    [field: SerializeField]
    public ScreenConfig Config { get; private set; }
}
