using UnityEngine;

[CreateAssetMenu(fileName = "Match Collactable Info", menuName = "Parrot Code/Config/Match Collactable Config")]
public sealed class MatchCollactableInfoConfig : ScreenContentConfig
{
    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField, Space(5)]
    public string Info { get; private set; }

    [field: SerializeField, Space(5)]
    public Sprite Thumbnail { get; private set; }
}
