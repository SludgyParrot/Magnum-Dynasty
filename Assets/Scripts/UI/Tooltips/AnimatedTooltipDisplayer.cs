using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class AnimatedTooltipDisplayer : TooltipDisplayer
{
    [SerializeField]
    private TooltipType tooltip;

    [SerializeField, Space(5)]
    private Animator animation;

    [SerializeField, Space(5)]
    private string animationTriggerEvent;

    [SerializeField, Space(5)]
    private UnityEvent<string> onPrefixValueChangedEvent = new UnityEvent<string>(), 
        onSuffixValueChangedEvent = new UnityEvent<string>();

    [SerializeField, Space(5)]
    private UnityEvent<Color> onPrefixForegroundColorChangedEvent = new UnityEvent<Color>(),
        onPrefixBackgroundColorChangedEvent = new UnityEvent<Color>();

    [SerializeField, Space(5)]
    private UnityEvent<Color> onSuffixForegroundColorChangedEvent = new UnityEvent<Color>(),
        onSuffixBackgroundColorChangedEvent = new UnityEvent<Color>();

    private void Start()
    {
        if(animation == null)
        {
            animation = GetComponent<Animator>();

            if(animation == null)
                throw new ArgumentNullException(nameof(animation), $"Cannot be null for: {gameObject.name}");
        }
    }

    public override void OnDisplayerEvent(ScreenTextConfig config)
    {
        if(config is AnimatedTooltipConfig tooltipConfig)
        {
            if(tooltipConfig.Tooltip != tooltip) return;

            var tooltipData = tooltipConfig.GetSplitTitle();

            if (!tooltipData.success) return;

            onPrefixValueChangedEvent?.Invoke(tooltipData.prefix);
            onSuffixValueChangedEvent?.Invoke(tooltipData.suffix);

            if(tooltipConfig.ConfigureColors)
            {
                onPrefixForegroundColorChangedEvent?.Invoke(tooltipConfig.PrefixColors.ForeGroundColor);
                onPrefixBackgroundColorChangedEvent?.Invoke(tooltipConfig.PrefixColors.BackgroundColor);

                onSuffixForegroundColorChangedEvent?.Invoke(tooltipConfig.SuffixColors.ForeGroundColor);
                onSuffixBackgroundColorChangedEvent?.Invoke(tooltipConfig.SuffixColors.BackgroundColor);
            }

            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        if (string.IsNullOrWhiteSpace(animationTriggerEvent))
        {
            Debug.LogWarning($"{nameof(animationTriggerEvent)} cannot be null or consist entirely of what space.");
            return;
        }

        animation.SetTrigger(animationTriggerEvent);
    }
}
