using UnityEngine;

public sealed class WaitForAudioPlayerNode : DefaultStoryNode
{
    [field: SerializeField]
    public AudioType Audio { get; private set; }
}
