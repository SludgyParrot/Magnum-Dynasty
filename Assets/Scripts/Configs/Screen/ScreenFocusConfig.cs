using UnityEngine;

[CreateAssetMenu(fileName = "Screen Focus Config", menuName = "Parrot Code/Config/Screen/Screen Focus Config")]
public sealed class ScreenFocusConfig : ScreenConfig
{
    [field: SerializeField, Space(5)]
    public float TransitionSpeed { get; private set; }
}
