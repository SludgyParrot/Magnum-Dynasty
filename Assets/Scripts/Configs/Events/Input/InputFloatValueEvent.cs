using UnityEngine;

[CreateAssetMenu(fileName = "Input Float Event", menuName = "Parrot Code/Config/Events/Input Float Event")]
public sealed class InputFloatValueEvent : InputEvent
{
    [field: SerializeField]
    public InputFloat Input;

    public float Value { get; private set; }

    public void SetValue(float value)
        => Value = value;
}
