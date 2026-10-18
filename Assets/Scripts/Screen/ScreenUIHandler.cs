using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ScreenUIHandler : MonoBehaviour
{
    [SerializeField]
    private ScreenUIType type;

    [SerializeField, Space(5)]
    private List<UITranslatableComponentHandler> translatableComponents = 
        new List<UITranslatableComponentHandler>();

    [SerializeField, Space(5)]
    private bool autoHide;

    [SerializeField, Space(5)]
    private float autoHideDelayDuration;

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnScreenEvent);

    private void OnDisable()
       => EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnScreenEvent);

    private void Start()
    {
        if(translatableComponents.Count == 0 )
            translatableComponents = GetComponentsInChildren<UITranslatableComponentHandler>().ToList();

        foreach(var component in translatableComponents)
            component.Initialize();
    }

    private void OnScreenEvent(EventConfig config)
    {
        if (config == null) return;

        if(config is ScreenUIEventConfig screenEvent)
        {
            if (screenEvent.Type == type)
                OnScreenState(screenEvent);
        }
    }

    private async void OnScreenState(ScreenUIEventConfig screenEvent)
    {
        if (translatableComponents.Count == 0) return;
        var tasks = translatableComponents.Select(x => x.OnTransitionAsync(screenEvent.State)).ToArray();
        await Task.WhenAll(tasks);

        if(autoHide)
        {
            if (screenEvent.State == UIViewState.Hide) return;

            await Task.Delay(autoHideDelayDuration.ToMilliseconds());
            var autoHideTasks = translatableComponents.Select(x => x.OnTransitionAsync(UIViewState.Hide)).ToArray();
            await Task.WhenAll(autoHideTasks);
        }
    }
}
