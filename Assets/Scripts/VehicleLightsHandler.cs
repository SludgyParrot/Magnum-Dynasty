using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class VehicleLight
{
    [SerializeField]
    private string name;

    [SerializeField, Space(5)]
    private Material lightMaterial;

    [SerializeField, Space(5)]
    private VehicleLightType lightType;

    public Material LightMaterial => lightMaterial;
    public VehicleLightType LightType => lightType;

    public void ToggleLight(bool isOn)
    {
        if (isOn)
            LightMaterial.EnableKeyword("_EMISSION");
        else
            LightMaterial.DisableKeyword("_EMISSION");
    }
}

public sealed class VehicleLightsHandler : MonoBehaviour
{
    [SerializeField]
    private List<VehicleLight> lights = 
        new List<VehicleLight>();

    public void ToggleLight(VehicleLightType lightType, bool isOn)
    {
        foreach (VehicleLight light in lights)
        {
            if (light.LightType != lightType) continue;         
            light.ToggleLight(isOn);
        }
    }
}
