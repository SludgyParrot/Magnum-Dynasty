using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PositioningSystem : MonoBehaviour
{
    [SerializeField]
    private List<Vehicle> vehicles = new List<Vehicle>();

    public int GetPosition(Vehicle vehicle)
    {
        vehicles.Sort((x, y) => y.PathIndex.CompareTo(x.PathIndex));

        return vehicles.IndexOf(vehicle) + 1;
    }
}
