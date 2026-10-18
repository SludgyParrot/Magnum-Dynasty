using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match Animated Screen Config", menuName = "Parrot Code/Config/Match Animated Screen")]
public sealed class AnimatedTooltipConfig : ScreenTextConfig
{
    [field: SerializeField]
    public string Title { get; private set; }

    [field: SerializeField, Space(5)]
    public TooltipType Tooltip { get; private set; }

    [field: SerializeField, Space(5)]
    public bool ConfigureColors { get; private set; }

    [field: SerializeField, Space(5)]
    public ColorConfig PrefixColors { get; private set; }

    [field: SerializeField, Space(5)]
    public ColorConfig SuffixColors { get; private set; }

    public (string prefix, string suffix, bool success) GetSplitTitle()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return (string.Empty, string.Empty, false);

        var splitTitle = Title.Split(' ');

        if(splitTitle != null && splitTitle.Length == 2)
            return (splitTitle[0], splitTitle[1], true);
        else
        {
            var stringLength = Title.Length;
            var midLength = Mathf.FloorToInt(stringLength / 2);
            var prefix = Title.Substring(0, midLength);
            var suffix = Title.Substring(midLength + 1, stringLength - 1);

            if (!string.IsNullOrWhiteSpace(suffix) && !string.IsNullOrWhiteSpace(prefix))
                return (prefix, suffix, true);
            else
                return (string.Empty, string.Empty, false);
        }
    }
}
