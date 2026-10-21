using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

public sealed class StoryGraphManager : SingletonInstance<StoryGraphManager>
{
    [SerializeField]
    private List<StoryGraph> graphs = 
        new List<StoryGraph>();

    [SerializeField, Space(5), 
        Tooltip("Do not assign a value here. " +
        "This is generated during runtime.")]
    private StoryGraph currentGraph;

    [SerializeField, Space(5), 
        Tooltip("Do not assign a value here. " +
        "This is generated during runtime.")]
    private StoryGraphNode currentGraphNode;

    [SerializeField, Space(5)]
    private UnityEvent onGraphStartedEvent, 
        onGraphExitedEvent, 
        onGraphCompletedEvent;

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnGraphEntry);

    private void OnDisable()
    {
        EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnGraphEntry);
        cancellationTokenSource.Cancel();
    }

    private void Start()
    {
        if(graphs.Count == 0) return;
        InitializeGraphs();
    }

    private void InitializeGraphs()
    {
        foreach (var graph in graphs) 
            graph.Initialize();
    }

    private void OnGraphEntry(EventConfig entry)
    {
        if(entry == null || graphs.Count == 0) return;

        var storyGraph = GetStoryGraph(entry);

        if (!string.IsNullOrWhiteSpace(storyGraph.errorMessage)) return;

        if (storyGraph.story.Completed || 
            storyGraph.story.HasPrerequisiteGraphs || (currentGraph != null && storyGraph.story == currentGraph)) return;

        currentGraph = storyGraph.story;

        ExecuteGraphNode(currentGraph.Entry);
    }

    private async void ExecuteGraphNode(StoryGraphNode node)
    {
        currentGraphNode = node;

        switch(node)
        {
            case EntryNode entry:
                ExecuteNextGraphNode(node);
                break;
            case TooltipNode tooltip:
                if(tooltip.Config == null)
                    throw new ArgumentNullException(nameof(tooltip.Config), "Cannot be null for tooltip.");
                EventsManager.Instance.OnDisplayTooltipEvent?.Invoke(tooltip.Config);
                ExecuteNextGraphNode(node);
                break;
            case TriggerEventsNode triggerEvent:
                if (triggerEvent.Events != null && triggerEvent.Events.Count > 0)
                {
                    foreach (var eventConfig in triggerEvent.Events)
                        EventsManager.Instance.OnTriggeredEvent?.Invoke(eventConfig);
                    ExecuteNextGraphNode(node);
                }
                else
                    Debug.LogError($"Trigger events failed. There are no events assigned for {nameof(triggerEvent.Events)}");
                break;
            case ScreenEventsNode screenEvents:
                if(screenEvents.Events != null &&  screenEvents.Events.Count > 0)
                {
                    foreach (var eventConfig in screenEvents.Events)
                        EventsManager.Instance.OnTriggeredEvent?.Invoke(eventConfig);
                    ExecuteNextGraphNode(node);
                }
                else
                    Debug.LogError($"Send screen event failed. There are no events assigned for {nameof(screenEvents.Events)}");
                break;
            case WaitForSecondsNode waitForSeconds:
                await Task.Delay(waitForSeconds.Seconds.ToMilliseconds());
                ExecuteNextGraphNode(node);
                break;
            case WaitForEventsNode waitForEvents:
                List<EventConfig> triggeredEvents = new List<EventConfig>();
                bool eventsTriggered = false;
                float eventsTimeOut = waitForEvents.TimeOut;
                bool eventHasATimeOut = eventsTimeOut > 0;
                bool eventTimedOut = false;
                while (!eventsTriggered &&
                    !cancellationTokenSource.Token.IsCancellationRequested)
                {
                    eventsTriggered = triggeredEvents.Count == waitForEvents.Events.Count;

                    if (eventHasATimeOut)
                    {
                        eventsTimeOut -= 1 * Time.unscaledDeltaTime;
                        eventTimedOut = eventsTimeOut <= 0.0f;
                        if (eventTimedOut)
                            eventsTriggered = true;
                    }
                    foreach (EventConfig eventConfig in waitForEvents.Events)
                    {
                        if (!EventsManager.Instance.IsCurrentEvent(eventConfig)) continue;

                        if(!triggeredEvents.Contains(eventConfig))
                            triggeredEvents.Add(eventConfig);
                    }
                    await Task.Yield();
                }
                if(eventTimedOut)
                    ExecuteNextGraphNode(node, waitForEvents.TimedOutEventName);
                else
                    ExecuteNextGraphNode(node);
                break;
            case WaitForAudioPlayerNode waitForAudioPlayer:
                ExecuteNextGraphNode(node);
                break;
            case WaitForInputNode waitForInput:
                if (waitForInput.InputActions.Count > 0)
                {
                    bool inputTriggered = false;
                    float timeOut = waitForInput.TimeOut;
                    bool hasTimeOut = timeOut > 0;
                    bool timedOut = false;
                    while (!inputTriggered && !cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if(hasTimeOut)
                        {
                            timeOut -= 1 * Time.unscaledDeltaTime;
                            timedOut = timeOut <= 0.0f;

                            if (timedOut)
                                inputTriggered = true;
                        }

                        for (int i = 0; i < waitForInput.InputActions.Count; i++)
                        {
                            var eventConfig = waitForInput.InputActions[i];
                            if (!EventsManager.Instance.IsCurrentEvent(eventConfig)) continue;

                            switch (eventConfig)
                            {
                                case InputActionEvent action:
                                    var currentInputAction = EventsManager.Instance.CurrentTriggeredEventConfig as InputActionEvent;
                                    inputTriggered = action.Input == currentInputAction.Input;
                                    break;
                                case InputFloatValueEvent floatValue:
                                    var currentInputFloat = EventsManager.Instance.CurrentTriggeredEventConfig as InputFloatValueEvent;
                                    inputTriggered = floatValue.Input == currentInputFloat.Input;
                                    break;
                                case InputVectorValueEvent vectorValue:
                                    var currentInputVector = EventsManager.Instance.CurrentTriggeredEventConfig as InputVectorValueEvent;
                                    inputTriggered = vectorValue.Input == currentInputVector.Input;
                                    break;
                                default:
                                    Debug.LogError($"{nameof(eventConfig)} is currently not supported by this version of await input node.");
                                    break;
                            }
                        }

                        await Task.Yield();
                    }

                    if (timedOut)
                        ExecuteNextGraphNode(node, waitForInput.TimedOutEventName);
                    else
                        ExecuteNextGraphNode(node);
                }
                else
                    Debug.LogError($"Wait for event failed. There are no events assigned for {nameof(waitForInput.InputActions)}");
                break;
            case ScreenStateNode screen:
                EventsManager.Instance.OnScreenConfigurationEvent?.Invoke(screen.Config);
                ExecuteNextGraphNode(node);
                break;
            case MusicPlayerNode musicPlayer:
                EventsManager.Instance.OnTriggeredEvent?.Invoke(musicPlayer.PlayerEvent);
                ExecuteNextGraphNode(node);
                break;
            case GameTimeNode gameTime:
                Time.timeScale = gameTime.Time;
                ExecuteNextGraphNode(node);
                break;
            case ExitNode exit:
                if(exit.ExitGraph != null)
                {
                    currentGraph = exit.ExitGraph;
                    ExecuteNextGraphNode(exit.ExitGraph.Entry);
                }
                break;
            case CompleteNode complete:
                currentGraph.CompleteGraph();
                break;
        }
    }

    private void ExecuteNextGraphNode(StoryGraphNode node, 
        string output = "output")
    {
        var port = node.Ports.FirstOrDefault(x => x.IsOutput && x.fieldName == output);

        if (port == null || port.ConnectionCount == 0) return;

        var connectedNode = port.Connection.node as StoryGraphNode;

        if (connectedNode == null) return;

        ExecuteGraphNode(connectedNode);
    }

    private (StoryGraph story, string errorMessage) GetStoryGraph(EventConfig entryEvent)
    {
        StoryGraph story = graphs.FirstOrDefault(x => x.Entry.EntryEvents.Contains(entryEvent));
        string errorMessage = story == null? $"{nameof(story)} of entry event: {entryEvent} was not found in the graphs for: {gameObject.name}" : string.Empty;
        return (story, errorMessage);
    }
}
