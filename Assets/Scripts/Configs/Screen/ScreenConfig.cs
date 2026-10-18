using UnityEngine;

public abstract class ScreenConfig : ScriptableObject
{
    [field: SerializeField]
    public ScreenState State { get; private set; }
}
