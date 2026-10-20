using System.Collections.Generic;
using UnityEngine;

public sealed class WaitForInputNode : TimeOutBaseNode
{
   [field: SerializeField, Space(5)]
   public List<InputEvent> InputActions {  get; private set; }
}
