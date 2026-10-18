using UnityEngine;
using TMPro;
using UnityEngine.UI;

public sealed class MusicUIHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI artistNameDisplayer;

    [SerializeField]
    private int maxSongTitleLength = 20,
        maxArtistNameLength = 20;

    [SerializeField, Space(5)]
    private TextMeshProUGUI songNameDisplayer;

    [SerializeField, Space(5)]
    private TextMeshProUGUI songYearDisplayer;

    [SerializeField, Space(5)]
    private Image albumArtDisplayer;

    [SerializeField, Space(5)]
    private ScreenUIEventConfig playerEvent;

    private const int WORD_PADDING_COUNT = 3;

    public void ShowDisplayer(MusicConfig config)
    {
        string artistName = config.Artist;
        string songTitle = config.Title;

        if(artistName.Length > maxArtistNameLength)
        {
            var newName = artistName.Substring(0, maxArtistNameLength - WORD_PADDING_COUNT) + "...";
            artistName = newName;
        }

        if(songTitle.Length > maxSongTitleLength)
        {
            var newTitle = songTitle.Substring(0, maxSongTitleLength - WORD_PADDING_COUNT) + "...";
            songTitle = newTitle;
        }

        artistNameDisplayer.SetText(artistName);
        songNameDisplayer.SetText(songTitle);
        songYearDisplayer.SetText(config.Year);

        if(playerEvent != null)
            EventsManager.Instance.OnTriggeredEvent?.Invoke(playerEvent);
    }
}
