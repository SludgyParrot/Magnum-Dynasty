using UnityEngine;
using UnityEngine.Events;

public sealed class EventsManager : SingletonInstance<EventsManager>
{
    [SerializeField]
    public UnityEvent<float, float> OnSpeedOMeterValueChanged = 
        new UnityEvent<float, float>();

    public void OnSceneExit()
    {
        OnSpeedOMeterValueChanged?.RemoveAllListeners();
    }
}
