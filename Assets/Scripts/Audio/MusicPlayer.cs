using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class MusicGroup
{
    [field: SerializeField]
    public string GroupName { get; private set; }

    [field: SerializeField, Space(5)]
    public MusicGroupType GroupType {  get; private set; }

    [field: SerializeField, Space(5)]
    public List<MusicConfig> Music {  get; private set; } = 
        new List<MusicConfig>();

    [field: SerializeField, Space(5)]
    public bool Randomize { get; private set; }

    public int RandomSongIndex => UnityEngine.Random.Range(0, Music.Count - 1);
    private int currentSongIndex;

    public MusicConfig GetSong()
    {
        if(Randomize)
        {
            var randomValue = RandomSongIndex;

            while(randomValue == currentSongIndex)
                randomValue = RandomSongIndex;

            currentSongIndex = randomValue;
            return Music[currentSongIndex];
        }

        MusicConfig config = Music[currentSongIndex];

        if(currentSongIndex < Music.Count - 1)
            currentSongIndex++;
        else
            currentSongIndex = 0;

        return config;
    }

    public MusicConfig GetSong(bool randomize)
    {
        Randomize = randomize;
        return GetSong();
    }
}

public sealed class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private MusicGroupType currentGroup;

    [SerializeField, Space(5)]
    private List<MusicGroup> musicGroups = 
        new List<MusicGroup>();

    [SerializeField, Space(5)]
    private bool autoPlay;

    [SerializeField, Space(5)]
    private float autoPlayDelayTime;

    [SerializeField, Space(5)]
    private UnityEvent<MusicConfig> onSongStartedEvent, 
        onSongCompletedEvent;

   public MusicGroup CurrentMusicGroup { get; private set; }

    private void OnEnable()
    {
        AudioManager.Instance.OnPlayerStartedEvent.AddListener(OnSongStartedEvent);
        AudioManager.Instance.OnPlayerCompletedEvent.AddListener(OnSongCompletedEvent);
    }

    private void OnDisable()
    {
        AudioManager.Instance.OnPlayerStartedEvent.RemoveListener(OnSongStartedEvent);
        AudioManager.Instance.OnPlayerCompletedEvent.RemoveListener(OnSongCompletedEvent);
    }

    private void Start()
    {
        if (!autoPlay) return;
        Invoke("Initialize", autoPlayDelayTime);
    }

    private void Initialize()
        => PlayMusic(currentGroup);

    public void PlayMusic(MusicGroupType groupType)
    {
        currentGroup = groupType;

        var groupResults = GetMusicGroup(groupType);

        if(!string.IsNullOrWhiteSpace(groupResults.errorMessage))
        {
            Debug.LogError(groupResults.errorMessage);
            return;
        }

        CurrentMusicGroup = groupResults.group;
        AudioManager.Instance.PlayAudio(CurrentMusicGroup.GetSong());
    }

    private void OnSongStartedEvent(AudioType player, AudioConfig config)
    {
        if (player != AudioType.Music) return;

        if (config is MusicConfig music)
            onSongStartedEvent?.Invoke(music);
    }

    private void OnSongCompletedEvent(AudioType player, AudioConfig config)
    {
        if (player != AudioType.Music) return;

        if (config is MusicConfig music)
            AudioManager.Instance.PlayAudio(CurrentMusicGroup.GetSong());
    }

    public (MusicGroup group, string errorMessage) GetMusicGroup(MusicGroupType groupType)
    {
        MusicGroup group = musicGroups.FirstOrDefault(x => x.GroupType == groupType);
        string errorMessage = group == null ? $"Couldn't find music group of type: {groupType}" : string.Empty;
        return (group, errorMessage);
    }
}
