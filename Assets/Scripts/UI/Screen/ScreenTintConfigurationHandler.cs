using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public sealed class ScreenTintConfigurationHandler : ScreenConfiguration
{
    [SerializeField]
    private Transform screenRootParent;

    [SerializeField, Space(5)]
    private List<ScreenSortingParentLayer> sortingLayers = 
        new List<ScreenSortingParentLayer>();

    [SerializeField, Space(5)]
    private ScreenSortingLayer defaultSortingLayer;

    private Image tintImage;
    private CanvasGroup tintGroup;

    public override void Initialize()
    {
        tintImage = GetComponent<Image>();
        tintGroup = GetComponent<CanvasGroup>();

        if (screenRootParent == null)
            screenRootParent = transform;

        OnReset();
    }

    public override void OnScreenConfigurationEvent(ScreenConfig eventConfig)
    {
        if(eventConfig == null) return;

        if(eventConfig is ScreenTintConfig tintConfig)
            OnScreenTintConfiguration(tintConfig);
    }

    private void OnScreenTintConfiguration(ScreenTintConfig tintConfig)
    {
        SetSortingLayer(tintConfig.SortingLayer);
        ConfigureTintImage(tintConfig);
        ConfigureTintGroup(tintConfig);
    }

    private void ConfigureTintImage(ScreenTintConfig tintConfig)
    {
        if(tintConfig.State == ScreenState.Focus) return;

        tintImage.sprite = tintConfig?.OverlayImage;
        tintImage.color = tintConfig.OverlayTintColor;
    }

    private async void ConfigureTintGroup(ScreenTintConfig tintConfig)
    {
        bool state = tintConfig.State != ScreenState.Focus;

        tintGroup.interactable = state;
        tintGroup.blocksRaycasts = state;

        await OnScreenTransitionAsync(tintConfig.State, tintConfig.TransitionSpeed);
    }

    private async Task OnScreenTransitionAsync(ScreenState state, float speed)
    {
        float opacity = state == ScreenState.Focus? 0.0f : 1.0f;

        while (!Mathf.Approximately(tintGroup.alpha, opacity))
        {
            tintGroup.alpha = Mathf.MoveTowards(tintGroup.alpha, opacity, speed * Time.deltaTime);
            await Task.Yield();
        }

        tintGroup.alpha = opacity;
    }

    private void SetSortingLayer(ScreenSortingLayer layer)
    {
        var sortingLayerResults = GetSortingLayer(layer);

        if (string.IsNullOrWhiteSpace(sortingLayerResults.errorMessage))
            sortingLayerResults.layer.Add(transform, false);
    }

    private (ScreenSortingParentLayer layer, string errorMessage) GetSortingLayer(ScreenSortingLayer sortingLayer)
    {
        ScreenSortingParentLayer layer = sortingLayers.FirstOrDefault(x => x.Layer == sortingLayer);
        string errorMessage = layer == null ? $"{nameof(layer)} was not found for sorting layer: {sortingLayer}" : string.Empty;
        return (layer, errorMessage);
    }

    private void OnReset()
    {
        tintGroup.alpha = 0.0f;
        SetSortingLayer(defaultSortingLayer);
    }
}
