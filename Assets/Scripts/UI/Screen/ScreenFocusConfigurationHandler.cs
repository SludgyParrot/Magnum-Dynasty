using System;
using System.Threading.Tasks;
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

    private async void OnScreenFocusConfiguration(ScreenFocusConfig focusConfig)
    {
        if(focusConfig.State == ScreenState.Blur)
        {
            depthOfField.active = true;
            depthOfField.focusDistance.Override(focusDistance);
        }

        await OnScreenTransitionAsync(focusConfig.State, focusConfig.TransitionSpeed);
    }

    private async Task OnScreenTransitionAsync(ScreenState state, float speed)
    {
        float targetFocalLength = state == ScreenState.Focus ? 0.0f : focalLength;

        while(!Mathf.Approximately(depthOfField.focalLength.value, targetFocalLength))
        {
            depthOfField.focalLength.value = Mathf.MoveTowards(depthOfField.focalLength.value,
                targetFocalLength, speed * Time.deltaTime);

            await Task.Yield();
        }

        depthOfField.focalLength.Override(targetFocalLength);

        if(state == ScreenState.Focus)
            depthOfField.active = false;
    }
}
