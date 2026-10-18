using UnityEngine;

public sealed class GameTimeNode : DefaultStoryNode
{
    [field: SerializeField, Range(0.0f, 1.0f)]
    public float Time { get; private set; } = 1.0f;
}
