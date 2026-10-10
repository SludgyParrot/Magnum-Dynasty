using UnityEngine;


public abstract class AudioConfig : ScriptableObject
{
    [field: SerializeField]
    public AudioClip Clip { get; private set; }

    [field: SerializeField, Space(5)]
    public float Volume { get; private set; }

    [field: SerializeField, Space(5)]
    public bool Loop { get; private set; }

    [field: SerializeField, Space(5)]
    public float Delay { get; private set; }
}
