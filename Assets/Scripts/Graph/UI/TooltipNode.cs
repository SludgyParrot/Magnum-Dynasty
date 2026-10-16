using UnityEngine;

public sealed class TooltipNode : DefaultStoryNode
{
    [field: SerializeField]
    public ScreenTextConfig Config { get; private set; }
}
