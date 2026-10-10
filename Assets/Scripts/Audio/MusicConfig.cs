using UnityEngine;

[CreateAssetMenu(fileName = "Music Config", menuName = "Parrot Code/Config/Audio/Music")]
public sealed class MusicConfig : AudioConfig
{
    [field: SerializeField, Space(5)]
    public string Title { get; private set; }

    [field: SerializeField, Space(5)]
    public string Artist { get; private set; }

    [field: SerializeField, Space(5)]
    public string Album { get; private set; }

    [field: SerializeField, Space(5)]
    public string Composer { get; private set; }

    [field: SerializeField, Space(5)]
    public string Year { get; private set; }

    [field: SerializeField, Space(5)]
    public string Description { get; private set; }

    [field: SerializeField, Space(5)]
    public Sprite AlbumArt { get; private set; }
}
