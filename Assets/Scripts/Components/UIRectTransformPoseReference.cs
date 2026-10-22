using System;
using UnityEngine;

[Serializable]
public sealed class UIRectTransformPoseReference : UITransformPoseReference
{
    [field: SerializeField, Space(5)]
    public UIViewState State { get; private set; }
}
