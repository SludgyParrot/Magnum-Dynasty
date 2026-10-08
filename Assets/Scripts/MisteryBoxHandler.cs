using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public sealed class MisteryBoxHandler : MonoBehaviour
{
    [SerializeField]
    private MisteryBoxType boxType;
    [SerializeField, Space(5)]
    private int value;

    [SerializeField, Space(5)]
    private UnityEvent onBoxTriggeredEvent;

   private void OnTriggerEnter(Collider other)
    {
        Vehicle vehicle = other.GetComponent<Vehicle>();
        if (vehicle == null) return;

        vehicle.ApplyMisteryBox(boxType, value);
        onBoxTriggeredEvent?.Invoke();
    }
}
