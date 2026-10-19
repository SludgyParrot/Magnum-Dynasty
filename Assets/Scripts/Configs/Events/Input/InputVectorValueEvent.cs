using UnityEngine;

[CreateAssetMenu(fileName = "Input Vector Event", menuName = "Parrot Code/Config/Events/Input Vector Event")]
public sealed class InputVectorValueEvent : InputEvent
{
    [field: SerializeField]
    public InputVector Input { get; private set; }

    public Vector2 Value { get; private set; }

    public void SetValue(Vector2 value)
        => Value = value;
}
