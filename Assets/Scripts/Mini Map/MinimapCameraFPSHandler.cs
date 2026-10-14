using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class MinimapCameraFPSHandler : MonoBehaviour
{
    [SerializeField]
    private int targetFPS = 15;

    private float interval, 
        elapsed;

    private Camera miniMapCamera;

    private void Start()
    {
        miniMapCamera = GetComponent<Camera>();
        miniMapCamera.enabled = false;
        interval = 1.0f / targetFPS;
    }


    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > interval)
        {
            elapsed = 0;
            miniMapCamera.Render();
        }
    }
}
