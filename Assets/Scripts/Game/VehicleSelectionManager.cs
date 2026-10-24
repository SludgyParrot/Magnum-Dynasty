using UnityEngine;

public sealed class VehicleSelectionManager : SingletonInstance<VehicleSelectionManager>
{
    private Vehicle selectedVehicle;

    public void SelectVehicle(Vehicle vehicle)
        => selectedVehicle = vehicle;
}
