using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public sealed class MusicUIHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI artistNameDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI songNameDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI songYearDisplayer;

    [SerializeField, Space(5)]
    private Image albumArtDisplayer;

    private Animator animation;

    public Animator Animation 
    {
        get
        {
            if (animation == null) 
                animation = GetComponent<Animator>();
            return animation;
        }
    }

    public void ShowDisplayer(MusicConfig config)
    {
        artistNameDisplayer.SetText(config.Artist);
        songNameDisplayer.SetText(config.Title);
        songYearDisplayer.SetText(config.Year);

        Animation.SetTrigger("Show");
    }
}
