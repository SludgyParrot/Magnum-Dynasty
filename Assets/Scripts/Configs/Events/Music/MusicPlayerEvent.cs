using UnityEngine;

[CreateAssetMenu(fileName = "Music Player Event", menuName = "Parrot Code/Config/Events/Music Player Event")]
public sealed class MusicPlayerEvent : EventConfig
{
    [field: SerializeField]
    public MusicGroupType MusicGroup { get; private set; }

    [field: SerializeField, Space(5)]
    public float StartDelayDuration { get; private set; }
}
