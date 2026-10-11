using UnityEngine;
using TMPro;

public abstract class TextDisplayer : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI displayer;

    public virtual void SetText(string text)
        => displayer.SetText(text);
}
