using UnityEngine;

[CreateAssetMenu(fileName = "Player Selected Vehicle Event", menuName = "Parrot Code/Config/Events/Player Selected Vehicle Event")]
public sealed class PlayerSelectedVehicleEvent : EventConfig
{
    [field: SerializeField]
   public Vehicle SelectedVehicle { get; private set; }

    public void SetSelectedVehicle(Vehicle selectedVehicle)
        => SelectedVehicle = selectedVehicle;
}
