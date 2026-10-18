using System;
using UnityEngine;

[Serializable]
public sealed class ScreenSortingParentLayer
{
    [SerializeField]
    private string name;

    [field: SerializeField]
    public Transform Parent { get; private set; }

    [field: SerializeField]
    public ScreenSortingLayer Layer { get; private set; }

    public void Add(Transform item, bool worldPositionStays)
    {
        if (Parent == null) return;
        item.SetParent(Parent, worldPositionStays);
    }
}
