using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class InputTutorialUIViewHandler : UIView
{
    [SerializeField]
    private TextMeshProUGUI titleDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI infoDisplayer;

    [SerializeField, Space(5)]
    private Image iconDisplayer;

    [SerializeField, Space(5)]
    private Image backgroundDisplayer;

    public override void Configure(ScreenContentConfig screenContent)
    {
        if (screenContent == null) return;

        if (screenContent is InputTutorialConfig config)
        {
            if(!string.IsNullOrWhiteSpace(config.Title)
                && titleDisplayer != null)
                titleDisplayer.SetText(config.Title);

            if (!string.IsNullOrWhiteSpace(config.Info)
               && infoDisplayer != null)
                infoDisplayer.SetText(config.Info);

            if (iconDisplayer != null
              && config.Icon != null)
                iconDisplayer.sprite = config.Icon;

            if (backgroundDisplayer != null
                 && config.Background != null)
                backgroundDisplayer.sprite = config.Background;
        }
    }
}
