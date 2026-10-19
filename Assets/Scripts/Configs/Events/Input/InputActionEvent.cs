using UnityEngine;

[CreateAssetMenu(fileName = "Input Action Event", menuName = "Parrot Code/Config/Events/Input Action Event")]
public sealed class InputActionEvent : InputEvent
{
    [field: SerializeField]
    public InputButton Input { get; private set; }

    public bool Performed { get; private set; }

    public void SetPerformed(bool performed)
        => Performed = performed;
}
