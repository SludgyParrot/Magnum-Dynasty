using UnityEngine;

[CreateAssetMenu(fileName = "Input Tutorial Config", menuName = "Parrot Code/Config/Input Tutorial Config")]
public sealed class InputTutorialConfig : ScreenContentConfig
{
    [field: SerializeField]
    public string Title { get; private set; }

    [field: SerializeField, Space(5)]
    public string Info { get; private set; }

    [field: SerializeField, Space(5)]
    public Sprite Icon { get; private set; }

    [field: SerializeField, Space(5)]
    public Sprite Background { get; private set; }
}
