using UnityEngine;

[CreateAssetMenu(fileName = "Screen Tint Config", menuName = "Parrot Code/Config/Screen/Screen Tint Config")]
public sealed class ScreenTintConfig : ScreenConfig
{
    [field: SerializeField, Space(5)]
    public Sprite OverlayImage { get; private set; }

    [field: SerializeField, Space(5)]
    public Color OverlayTintColor { get; private set; } = 
        Color.white;

    [field: SerializeField, Space(5)]
    public float TransitionSpeed { get; private set; }

    [field: SerializeField, Space(5)]
    public ScreenSortingLayer SortingLayer { get; private set; }
}
