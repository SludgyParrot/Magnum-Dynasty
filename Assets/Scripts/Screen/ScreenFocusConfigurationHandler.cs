using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public sealed class ScreenFocusConfigurationHandler : ScreenConfiguration
{
    [SerializeField]
    private Volume volume;

    [SerializeField, Space(5)]
    private DepthOfFieldMode DOFMode = 
        DepthOfFieldMode.Bokeh;

    [SerializeField, Space(5)]
    private float focusDistance;

    [SerializeField, Space(5), Range(0.0f, 300.0f)]
    private float focalLength;

    [SerializeField, Space(5)]
    private DepthOfField depthOfField;

    public override void Initialize()
    {
        if(volume == null)
            throw new ArgumentNullException(nameof(volume),
                $"Cannot be null for: {gameObject.name}");

        if(volume.profile.TryGet<DepthOfField>(out var depthOfField))
            this.depthOfField = depthOfField;
        else
            throw new ArgumentNullException(nameof(this.depthOfField),
               $"Was not found in the rendering volume attached to: {gameObject.name}");

        this.depthOfField.mode.Override(DOFMode);
    }

    public override void OnScreenConfigurationEvent(ScreenConfig eventConfig)
    {
        if (eventConfig == null) return;

        if (eventConfig is ScreenFocusConfig focusConfig)
            OnScreenFocusConfiguration(focusConfig);
    }

    private void OnScreenFocusConfiguration(ScreenFocusConfig focusConfig)
    {
        if(focusConfig.State == ScreenState.Blur)
        {
            depthOfField.active = true;
            depthOfField.focusDistance.Override(focusDistance);
        }

        float targetFocalLength = focusConfig.State == ScreenState.Focus ? 0.0f : focalLength;
        depthOfField.focalLength.Override(targetFocalLength);

        if (focusConfig.State == ScreenState.Focus)
            depthOfField.active = false;
    }
}
