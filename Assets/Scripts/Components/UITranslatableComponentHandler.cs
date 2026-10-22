using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public sealed class UITranslatableComponentHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject layout;

    [SerializeField, Space(5)]
    private List<UIRectTransformPoseReference> translatePoseReferences =
        new List<UIRectTransformPoseReference>();

    [SerializeField, Space(5)]
    private UIViewState defaultState = 
        UIViewState.Hide;

    [SerializeField, Space(5)]
    private UnityEvent onShowTransitionStartedEvent = new UnityEvent(),
        onHideTransitionStartedEvent = new UnityEvent(),
        onShowTransitionEndedEvent = new UnityEvent(),
        onHideTransitionEndedEvent = new UnityEvent();

    [SerializeField, Space(5)]
    private float eventTriggerDelayDuration;

    private RectTransform view;
    private Canvas viewCanvas;
    private CanvasGroup viewGroup;

    private CancellationTokenSource cancellationTokenSource = 
        new CancellationTokenSource();

    private const float TRANSLATE_DISTANCE_THRESHOLD = 
        0.01f;

    private bool inTransition;

    private void OnDisable()
        => cancellationTokenSource.Cancel();

    public void Initialize()
    {
        if(layout == null) 
            throw new ArgumentNullException(nameof(layout), $"Cannot be null for: {gameObject.name}.");

        view = layout.GetComponent<RectTransform>();

        viewGroup = GetComponent<CanvasGroup>();
        viewCanvas = GetComponent<Canvas>();

        var referencePoseResults = GetReferencePose(defaultState);

        if (!string.IsNullOrWhiteSpace(referencePoseResults.errorMessage))
        {
            Debug.LogError(referencePoseResults.errorMessage);
            return;
        }

        var poseReference = referencePoseResults.reference;

        switch (poseReference.TransitionType)
        {
            case ScreenUITransitionType.Fade:
                viewGroup.alpha = defaultState == UIViewState.Show ? 1.0f : 0.0f;
                break;
            case ScreenUITransitionType.Translate:
                var anchoredPosition = poseReference.PoseOverrideRectTransform == null ? poseReference.AnchoredPosition : poseReference.PoseOverrideRectTransform.anchoredPosition;
                var rotation = poseReference.PoseOverrideRectTransform == null ? poseReference.Rotation : poseReference.PoseOverrideRectTransform.rotation;
                var sizeDelta = poseReference.PoseOverrideRectTransform == null ? poseReference.SizeDelta : poseReference.PoseOverrideRectTransform.sizeDelta;
                switch (poseReference.TranslateProperty)
                {
                    case TranslatePropertyType.Position:
                        view.anchoredPosition = anchoredPosition;
                        break;
                    case TranslatePropertyType.Rotation:
                        view.rotation = rotation;
                        break;
                    case TranslatePropertyType.Scale:
                        view.sizeDelta = sizeDelta;
                        break;
                    default:
                        view.anchoredPosition = anchoredPosition;
                        view.rotation = rotation;
                        view.sizeDelta = sizeDelta;
                        break;
                }
                break;
            default:
                SetUILayoutState(defaultState == UIViewState.Show);
                break;
        }
    }

    public async void OnTransition(UIViewState state)
        => await OnTransitionAsync(state);

    public async Task OnTransitionAsync(UIViewState state)
    {
        var referencePoseResults = GetReferencePose(state);

        if (!string.IsNullOrWhiteSpace(referencePoseResults.errorMessage))
        {
            Debug.LogError(referencePoseResults.errorMessage);
            return;
        }

        var poseReference = referencePoseResults.reference;

        await Task.Delay(poseReference.TransitionDelay.ToMilliseconds());

        if(state == UIViewState.Show) 
            onShowTransitionStartedEvent?.Invoke();
        else
            onHideTransitionStartedEvent?.Invoke();

        switch (poseReference.TransitionType)
        {
            case ScreenUITransitionType.Fade:
                await OnScreenUIFadeAsync(state,
                    poseReference.TransitionDuration);
                break;
            case ScreenUITransitionType.Translate:
                await OnScreenUITranslateAsync(poseReference);
                break;
            default:
                SetUILayoutState(state == UIViewState.Show);
                break;
        }

        if (state == UIViewState.Show)
            onShowTransitionEndedEvent?.Invoke();
        else
            onHideTransitionEndedEvent?.Invoke();
    }

    private async Task OnScreenUIFadeAsync(UIViewState state, float fadeSpeed)
    {
        float opacity = state == UIViewState.Show ? 1.0f : 0.0f;

        while (!Mathf.Approximately(viewGroup.alpha, opacity) && !cancellationTokenSource.IsCancellationRequested)
        {
            viewGroup.alpha = Mathf.MoveTowards(viewGroup.alpha,
                opacity,
                fadeSpeed * Time.unscaledDeltaTime);

            await Task.Yield();
        }

        viewGroup.alpha = opacity;

        if(state == UIViewState.Show) 
            onShowTransitionEndedEvent?.Invoke();
        else
            onHideTransitionEndedEvent?.Invoke();
    }

    private async Task OnScreenUITranslateAsync(UIRectTransformPoseReference reference)
    {
        var anchoredPosition = reference.PoseOverrideRectTransform == null ? reference.AnchoredPosition : reference.PoseOverrideRectTransform.anchoredPosition;
        var rotation = reference.PoseOverrideRectTransform == null ? reference.Rotation : reference.PoseOverrideRectTransform.rotation;
        var sizeDelta = reference.PoseOverrideRectTransform == null ? reference.SizeDelta : reference.PoseOverrideRectTransform.sizeDelta;

        switch (reference.TranslateProperty)
        {
            case TranslatePropertyType.Position:
                await TranslatePositionAsync(anchoredPosition,
                    reference.TransitionDuration, 
                    reference.CompletionDistanceThreshold);
                break;
            case TranslatePropertyType.Rotation:
                await TranslateRotationAsync(rotation, 
                    reference.TransitionDuration, 
                    reference.CompletionDistanceThreshold);
                break;
            case TranslatePropertyType.Scale:
                await TranslateScaleAsync(sizeDelta, 
                    reference.TransitionDuration,
                    reference.CompletionDistanceThreshold);
                break;
            default:
                var tasks = new List<Task>
                {
                    TranslatePositionAsync(anchoredPosition, reference.TransitionDuration, 
                    reference.CompletionDistanceThreshold),
                    TranslateRotationAsync(rotation, reference.TransitionDuration, 
                    reference.CompletionDistanceThreshold),
                    TranslateScaleAsync(sizeDelta, reference.TransitionDuration, 
                    reference.CompletionDistanceThreshold)
                };
                await Task.WhenAll(tasks);
                break;
        }
    }

    private async Task TranslatePositionAsync(Vector2 anchoredPosition,
        float translateSpeed,
        float translateDistanceThreshold = 0.01f)
    {
        if (view == null) return;

        while(inTransition &&
            !cancellationTokenSource.IsCancellationRequested)
            await Task.Yield();

        while ((view.anchoredPosition - anchoredPosition).magnitude > translateDistanceThreshold && 
            !cancellationTokenSource.IsCancellationRequested)
        {
            inTransition = true;
            view.anchoredPosition = Vector2.Lerp(view.anchoredPosition, anchoredPosition, translateSpeed * Time.unscaledDeltaTime);
            await Task.Yield();
        }

        view.anchoredPosition = anchoredPosition;
        inTransition = false;
    }

    private async Task TranslateRotationAsync(Quaternion rotation, 
        float translateSpeed,
        float translateDistanceThreshold = 0.01f)
    {
        if (view == null) return;

        while (inTransition &&
          !cancellationTokenSource.IsCancellationRequested)
            await Task.Yield();

        while ((view.eulerAngles - rotation.eulerAngles).magnitude > translateDistanceThreshold &&
            !cancellationTokenSource.IsCancellationRequested)
        {
            inTransition = true;
            view.eulerAngles = Vector3.Slerp(view.eulerAngles, rotation.eulerAngles, translateSpeed * Time.unscaledDeltaTime);
            await Task.Yield();
        }

        view.eulerAngles = rotation.eulerAngles;
        inTransition = false;
    }

    private async Task TranslateScaleAsync(Vector2 sizeDelta, 
        float translateSpeed, 
        float translateDistanceThreshold = 0.01f)
    {
        if (view == null) return;

        while (inTransition && 
            !cancellationTokenSource.IsCancellationRequested)
            await Task.Yield();

        while (Mathf.Abs((view.sizeDelta - sizeDelta).magnitude) > translateDistanceThreshold &&
            !cancellationTokenSource.IsCancellationRequested)
        {
            inTransition = true;
            view.sizeDelta = Vector2.Lerp(view.sizeDelta, sizeDelta, translateSpeed * Time.unscaledDeltaTime);
            await Task.Yield();
        }

        view.sizeDelta = sizeDelta;
        inTransition = false;
    }

    private (UIRectTransformPoseReference reference, string errorMessage) GetReferencePose(UIViewState state)
    {
        UIRectTransformPoseReference reference = translatePoseReferences.FirstOrDefault(x => x.State == state);
        string errorMessage = reference == null ? $"{reference} for state {state} was not found in the {nameof(translatePoseReferences)} for {gameObject.name}." : string.Empty;
        return (reference, errorMessage);
    }

    private void SetUILayoutState(bool show)
    {
        if(viewCanvas == null) 
            layout.SetActive(show);
        else
            viewCanvas.enabled = show;
    }
}
