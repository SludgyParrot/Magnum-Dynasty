using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

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

    private void OnEnable()
        => EventsManager.Instance.OnTriggeredEvent.AddListener(OnGraphEntry);

    private void OnDisable()
        => EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnGraphEntry);

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
        if(entry == null) return;

        var storyGraph = GetStoryGraph(entry);

        if (!string.IsNullOrWhiteSpace(storyGraph.errorMessage)) return;

        if (storyGraph.story.Completed || 
            storyGraph.story.HasPrerequisiteGraphs) return;

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
            case TriggerEventNode triggerEvent:
                EventsManager.Instance.OnTriggeredEvent?.Invoke(triggerEvent.Event);
                ExecuteNextGraphNode(node);
                break;
            case WaitForSecondsNode waitForSeconds:
                await Task.Delay(waitForSeconds.Seconds.ToMilliseconds());
                ExecuteNextGraphNode(node);
                break;
            case WaitForEventsNode waitForEvents:
                List<EventConfig> triggeredEvents = new List<EventConfig>();
                while (triggeredEvents.Count < waitForEvents.Events.Count)
                {
                    foreach (EventConfig eventConfig in waitForEvents.Events)
                    {
                        if (!EventsManager.Instance.IsCurrentEvent(eventConfig)) continue;

                        if(!triggeredEvents.Contains(eventConfig))
                            triggeredEvents.Add(eventConfig);
                    }
                    await Task.Yield();
                }
                ExecuteNextGraphNode(node);
                break;
            case WaitForAudioPlayerNode waitForAudioPlayer:
                ExecuteNextGraphNode(node);
                break;
            case WaitForInputNode waitForInput:
                ExecuteNextGraphNode(node);
                break;
            case ScreenNode screen:
                EventsManager.Instance.OnScreenConfigurationEvent?.Invoke(screen.Config);
                ExecuteNextGraphNode(node);
                break;
            case MusicPlayerNode musicPlayer:
                EventsManager.Instance.OnTriggeredEvent?.Invoke(musicPlayer.PlayerEvent);
                ExecuteNextGraphNode(node);
                break;
            case ExitNode exit:
                if(exit.ExitGraph != null)
                {
                    currentGraph = exit.ExitGraph;
                    ExecuteGraphNode(currentGraph.Entry);
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
        StoryGraph story = graphs.FirstOrDefault(x => x.Entry.EntryEvents.Contains(entryEvent) && x != currentGraph);
        string errorMessage = story == null? $"{nameof(story)} of entry event: {entryEvent} was not found in the graphs for: {gameObject.name}" : string.Empty;
        return (story, errorMessage);
    }
}
