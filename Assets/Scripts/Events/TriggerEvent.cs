using System.Threading.Tasks;
using UnityEngine;

public abstract class TriggerEvent : MonoBehaviour
{
    [SerializeField]
    private TriggerEventEntry triggerEvent;

    [SerializeField, Space(5)]
    private float delay;

    [SerializeField, Space(5)]
    private float duration, intervals;

    private void Awake()
    {
        if (triggerEvent == TriggerEventEntry.OnAwake)
            ExecuteEvent();

        if (triggerEvent == TriggerEventEntry.OnIntervals)
            InvokeRepeating("OnTriggerEvent", duration, intervals);
    }

    private void OnEnable()
    {
        if (triggerEvent == TriggerEventEntry.OnEnabled)
            ExecuteEvent();

        if (triggerEvent == TriggerEventEntry.OnIntervals)
            InvokeRepeating("OnTriggerEvent", duration, intervals);
    }

    private void OnDisable()
    {
        if (triggerEvent == TriggerEventEntry.OnDisabled)
            ExecuteEvent();

        if (triggerEvent == TriggerEventEntry.OnIntervals)
            InvokeRepeating("OnTriggerEvent", duration, intervals);
    }

    private void Start()
    {
        if (triggerEvent == TriggerEventEntry.OnStart)
            ExecuteEvent();

        if (triggerEvent == TriggerEventEntry.OnIntervals)
            InvokeRepeating("OnTriggerEvent", duration, intervals);
    }

    private async void ExecuteEvent()
    {
        if (delay > 0)
            await Task.Delay(delay.ToMilliseconds());

        Trigger();
    }

    public void OnTriggerEvent()
    {
        if (triggerEvent == TriggerEventEntry.Custom)
            ExecuteEvent();
    }

    public abstract void Trigger();

    public void OnReset()
        => CancelInvoke();
}
