using System.Collections.Generic;
using UnityEngine;

public sealed class WaitForInputNode : DefaultStoryNode
{
   [field: SerializeField]
   public List<InputEvent> InputActions {  get; private set; }
}
