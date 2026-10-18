using UnityEngine;

[CreateAssetMenu(fileName = "Screen Event Config", menuName = "Parrot Code/Config/Events/Screen Event Config")]
public sealed class ScreenUIEventConfig : EventConfig
{
    [field: SerializeField]
    public ScreenUIType Type { get; private set; }

    [field: SerializeField]
    public UIViewState State { get; private set; }
}
