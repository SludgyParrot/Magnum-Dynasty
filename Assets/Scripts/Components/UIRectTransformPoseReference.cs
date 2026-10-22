using System;
using UnityEngine;

[Serializable]
public sealed class UIRectTransformPoseReference
{
    [SerializeField]
    private string name;

    [field: SerializeField, Space(5)]
    public UIViewState State { get; private set; }

    [field: SerializeField, Space(5)]
    public Vector2 AnchoredPosition { get; private set; }

    [field: SerializeField, Space(5)]
    public Quaternion Rotation { get; private set; }

    [field: SerializeField, Space(5)]
    public Vector2 SizeDelta { get; private set; }

    [field: SerializeField, Space(5)]
    public RectTransform PoseOverrideRectTransform { get; private set; }

    [field: SerializeField, Space(5)]
    public TranslatePropertyType TranslateProperty { get; private set; }

    [field: SerializeField, Space(5)]
    public ScreenUITransitionType TransitionType { get; private set; }

    [field: SerializeField]
    public float TransitionDuration { get; private set; }

    [field: SerializeField]
    public float TransitionDelay { get; private set; }

    [field: SerializeField]
    public float CompletionDistanceThreshold { get; private set; } = 0.1f;
}
