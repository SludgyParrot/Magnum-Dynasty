using System;
using UnityEngine;

[Serializable]
public sealed class ColorConfig
{
    [SerializeField]
    private string name;

    [field: SerializeField, Space(5)]
    public Color ForeGroundColor { get; private set; } = 
        Color.white;

    [field: SerializeField, Space(5)]
    public Color BackgroundColor { get; private set; } =
        Color.black;
}
