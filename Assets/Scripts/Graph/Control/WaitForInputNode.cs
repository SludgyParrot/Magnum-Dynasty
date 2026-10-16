using UnityEngine;

public sealed class WaitForInputNode : DefaultStoryNode
{
   [field: SerializeField]
   public InputAction Input {  get; private set; }
}
