using UnityEngine;

[DisallowMultipleComponent]
public abstract class TooltipDisplayer : MonoBehaviour
{
    private void OnEnable()
        => EventsManager.Instance.OnDisplayTooltipEvent.AddListener(OnDisplayerEvent);

    private void OnDisable()
        => EventsManager.Instance.OnDisplayTooltipEvent.RemoveListener(OnDisplayerEvent);

    public abstract void OnDisplayerEvent(ScreenTextConfig config);
}
