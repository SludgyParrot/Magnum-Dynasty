using UnityEngine;

public sealed class ScreenNode : DefaultStoryNode
{
    [field: SerializeField]
    public ScreenConfig Config { get; private set; }
}
