using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class UITransformTween : MonoBehaviour
{
    [SerializeField]
    private RectTransform layout;

    [SerializeField, Space(5)]
    private AnimationType animationType;

    [SerializeField, Space(5)]
    private List<UITransformPoseReference> animationPoseReferences = 
        new List<UITransformPoseReference>();

    private CancellationTokenSource cancellationTokenSource;

    private bool inTransition;
    private int referenceIndex = 0;
    private int RandomReferenceIndex => Random.Range(0, animationPoseReferences.Count - 1);

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }

    private void Start()
    {
        if(layout == null)
            layout = GetComponent<RectTransform>();
    }

    public async void Play()
    {
        if (animationPoseReferences.Count == 0) return;

        cancellationTokenSource = new CancellationTokenSource();

        if (animationType == AnimationType.Randomized)
            referenceIndex = RandomReferenceIndex;

        await OnPlayAnimationAsync();
    }

    private async Task OnPlayAnimationAsync()
    {
        if (layout == null) return;

        Debug.Log($"~Starting to play {animationType} with distance: {(layout.sizeDelta - animationPoseReferences[referenceIndex].SizeDelta).magnitude}");

        if((layout.sizeDelta - animationPoseReferences[referenceIndex].SizeDelta).magnitude <= animationPoseReferences[referenceIndex].CompletionDistanceThreshold)
        {
            if (referenceIndex < animationPoseReferences.Count - 1)
                referenceIndex++;
            else
                referenceIndex = 0;
        }

        switch (animationType)
        {
            case AnimationType.Default:
                //await PlayDefaultAnimationAsync(cancellationTokenSource.Token);
                break;
            case AnimationType.Loop:
                await PlayLoopedScaleAnimationAsync(cancellationTokenSource.Token);
                //await PlayLoopedPositionAnimationAsync(cancellationTokenSource.Token);
                break;
            case AnimationType.Randomized:
                // await PlayRandomizedAnimationAsync(cancellationTokenSource.Token);
                break;
        }
    }

    private async Task PlayDefaultAnimationAsync(CancellationToken cancellation)
    {
        while(inTransition  &&
            !cancellation.IsCancellationRequested)
            await Task.Yield();

        await Task.Delay(animationPoseReferences[referenceIndex].TransitionDelay.ToMilliseconds());

        inTransition = true;

        while ((layout.anchoredPosition - animationPoseReferences[referenceIndex].AnchoredPosition).magnitude > animationPoseReferences[referenceIndex].CompletionDistanceThreshold && inTransition &&
            !cancellation.IsCancellationRequested)
        {
            layout.anchoredPosition = Vector2.Lerp(layout.anchoredPosition,
                animationPoseReferences[referenceIndex].AnchoredPosition, 
                animationPoseReferences[referenceIndex].TransitionDuration * Time.unscaledDeltaTime);

            if((layout.anchoredPosition - animationPoseReferences[referenceIndex].AnchoredPosition).magnitude <= animationPoseReferences[referenceIndex].CompletionDistanceThreshold)
            {
                await Task.Delay(animationPoseReferences[referenceIndex].TransitionDelay.ToMilliseconds());

                if (referenceIndex < animationPoseReferences.Count - 1)
                    referenceIndex++;
                else
                    inTransition = false;
            }

            await Task.Yield();
        }
    }

    private async Task PlayLoopedPositionAnimationAsync(CancellationToken cancellation)
    {
        while (inTransition &&
          !cancellation.IsCancellationRequested)
            await Task.Yield();

        await Task.Delay(animationPoseReferences[referenceIndex].TransitionDelay.ToMilliseconds());

        inTransition = true;

        while ((layout.anchoredPosition - animationPoseReferences[referenceIndex].AnchoredPosition).magnitude > animationPoseReferences[referenceIndex].CompletionDistanceThreshold && inTransition &&
            !cancellation.IsCancellationRequested)
        {
            layout.anchoredPosition = Vector2.Lerp(layout.anchoredPosition,
                animationPoseReferences[referenceIndex].AnchoredPosition,
                animationPoseReferences[referenceIndex].TransitionDuration * Time.unscaledDeltaTime);

            if ((layout.anchoredPosition - animationPoseReferences[referenceIndex].AnchoredPosition).magnitude <= animationPoseReferences[referenceIndex].CompletionDistanceThreshold)
            {
                if (referenceIndex < animationPoseReferences.Count - 1)
                    referenceIndex++;
                else
                    referenceIndex = 0;
            }

            await Task.Yield();
        }

        layout.anchoredPosition = animationPoseReferences[referenceIndex].AnchoredPosition;
    }


    private async Task PlayLoopedScaleAnimationAsync(CancellationToken cancellation)
    {
        while (inTransition &&
          !cancellation.IsCancellationRequested)
            await Task.Yield();

        await Task.Delay(animationPoseReferences[referenceIndex].TransitionDelay.ToMilliseconds());

        inTransition = true;

        Debug.Log($"~Playing Looped with distance: {(layout.sizeDelta - animationPoseReferences[referenceIndex].SizeDelta).magnitude}");

        while ((layout.sizeDelta - animationPoseReferences[referenceIndex].SizeDelta).magnitude > animationPoseReferences[referenceIndex].CompletionDistanceThreshold && inTransition &&
            !cancellation.IsCancellationRequested)
        {
            layout.sizeDelta = Vector2.Lerp(layout.sizeDelta,
                animationPoseReferences[referenceIndex].SizeDelta,
                animationPoseReferences[referenceIndex].TransitionDuration * Time.unscaledDeltaTime);

            if ((layout.sizeDelta - animationPoseReferences[referenceIndex].SizeDelta).magnitude <= animationPoseReferences[referenceIndex].CompletionDistanceThreshold)
            {
                await Task.Delay(animationPoseReferences[referenceIndex].TransitionDelay.ToMilliseconds());

                if (referenceIndex < animationPoseReferences.Count - 1)
                    referenceIndex++;
                else
                    referenceIndex = 0;
            }

            await Task.Yield();
        }

        layout.sizeDelta = animationPoseReferences[referenceIndex].SizeDelta;
    }

    private async Task PlayRandomizedAnimationAsync(CancellationToken cancellation)
    {
        while (inTransition &&
          !cancellation.IsCancellationRequested)
            await Task.Yield();


    }

    public void Stop()
    {
        if (cancellationTokenSource == null) return;

        cancellationTokenSource.Cancel();
        inTransition = false;
    }
}
