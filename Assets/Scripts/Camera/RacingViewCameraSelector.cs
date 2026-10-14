using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class CameraView
{
    [SerializeField]
    private string name;

    [SerializeField, Space(5)]
    private Camera camera;

    [SerializeField, Space(5)]
    private CameraViewType viewType;

    public CameraViewType ViewType => viewType;

    public void Select()
    {
        Debug.Log($"Selecting camera view: {viewType}");

        if (camera == null) return;
        camera.enabled = true;

        Debug.Log($"Selecting camera view: {viewType} succeeded");
    }

    public void Deselect()
    {
        Debug.Log($"Deselecting camera view: {viewType}");

        if (camera == null) return;
        camera.enabled = false;

        Debug.Log($"Deselecting camera view: {viewType} succeeded");
    }
}

[DisallowMultipleComponent]
public sealed class RacingViewCameraSelector : MonoBehaviour
{
    [SerializeField]
    private CameraViewType initialView = 
        CameraViewType.Racing;

    [SerializeField, Space(5)]
    private List<CameraView> cameraViews = 
        new List<CameraView>();

    private void Start()
    {
        if(cameraViews.Count == 0) return;
        SelectCameraView(initialView);
    }

    public void SelectCameraView(CameraViewType viewType)
    {

        Debug.Log($"Camera Switched to: {viewType}");

        foreach (CameraView view in cameraViews)
        {
            if(view == null) continue;

            if(view.ViewType == viewType)
                view.Select();
            else
                view.Deselect();
        }
    }
}
