using UnityEngine;

[DisallowMultipleComponent]
public abstract class ScreenConfiguration : MonoBehaviour
{
    private void OnEnable()
       => EventsManager.Instance.OnScreenConfigurationEvent.AddListener(OnScreenConfigurationEvent);

    private void OnDisable()
        => EventsManager.Instance.OnScreenConfigurationEvent.RemoveListener(OnScreenConfigurationEvent);

    private void Start()
        => Initialize();

    public abstract void Initialize();
    public abstract void OnScreenConfigurationEvent(ScreenConfig eventConfig);
}
