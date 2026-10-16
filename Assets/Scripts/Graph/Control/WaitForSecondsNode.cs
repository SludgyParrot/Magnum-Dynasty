using UnityEngine;

public sealed class WaitForSecondsNode : DefaultStoryNode
{
    [field: SerializeField]
    public float Seconds { get; private set; }
}
