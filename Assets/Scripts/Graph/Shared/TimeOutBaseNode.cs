using UnityEngine;

public abstract class TimeOutBaseNode : DefaultStoryNode
{
    [field: SerializeField]
    public float TimeOut { get; set; }

    public string TimedOutEventName { get; } = "timedOut";

    [Output]
    public int timedOut;
}
