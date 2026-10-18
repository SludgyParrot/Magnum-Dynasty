using UnityEngine;

public sealed class MusicPlayerNode : DefaultStoryNode
{
    [field: SerializeField]
    public MusicPlayerEvent PlayerEvent { get; private set; }
}
