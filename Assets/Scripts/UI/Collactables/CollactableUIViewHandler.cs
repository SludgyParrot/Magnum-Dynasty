using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class CollactableUIViewHandler : UIView
{
    [SerializeField]
    private TextMeshProUGUI itemNameDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI infoDisplayer;

    [SerializeField, Space(5)]
    private Image thumbnailDisplayer;

    public override void Configure(ScreenContentConfig screenContent)
    {
        if (screenContent == null) return;

        if(screenContent is MatchCollactableInfoConfig config)
        {
            if (!string.IsNullOrWhiteSpace(config.ItemName) && itemNameDisplayer != null)
                itemNameDisplayer.SetText(config.ItemName);

            if (!string.IsNullOrWhiteSpace(config.Info) && infoDisplayer != null)
                infoDisplayer.SetText(config.Info);

            if (thumbnailDisplayer != null && config.Thumbnail != null)
                thumbnailDisplayer.sprite = config.Thumbnail;
        }
    }
}
