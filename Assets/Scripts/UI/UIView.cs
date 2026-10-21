using UnityEngine;

[DisallowMultipleComponent]
public abstract class UIView : MonoBehaviour
{
    public abstract void Configure(ScreenContentConfig screenContent);
}
