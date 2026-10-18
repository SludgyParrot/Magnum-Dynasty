using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    private InputActionAsset inputAction;

    [SerializeField, Space(5)]
    private InputActionReference nextMusicInput, 
        prevMusicInput;

    [SerializeField, Space(5)]
    private UnityEvent<MusicConfig> onSongStartedEvent, 
        onSongCompletedEvent;

   public MusicGroup CurrentMusicGroup { get; private set; }

    private MusicConfig currentMusic,
        previousMusic;

    private Stack<MusicConfig> playedMusic = 
        new Stack<MusicConfig>();

    private void OnEnable()
    {
        AudioManager.Instance.OnPlayerStartedEvent.AddListener(OnSongStartedEvent);
        AudioManager.Instance.OnPlayerCompletedEvent.AddListener(OnSongCompletedEvent);
        EventsManager.Instance.OnTriggeredEvent.AddListener(OnTriggeredEvent);
    }

    private void OnDisable()
    {
        AudioManager.Instance.OnPlayerStartedEvent.RemoveListener(OnSongStartedEvent);
        AudioManager.Instance.OnPlayerCompletedEvent.RemoveListener(OnSongCompletedEvent);
        EventsManager.Instance.OnTriggeredEvent.RemoveListener(OnTriggeredEvent);
    }

    private void Start()
    {
        if(inputAction != null)
        {
            inputAction.Enable();

            if(nextMusicInput != null)
                nextMusicInput.action.performed += PlayNextSong;

            if(prevMusicInput != null)
                prevMusicInput.action.performed += PlayePreviousSong;
        }

        if (!autoPlay) return;
        Invoke("Initialize", autoPlayDelayTime);
    }

    private async void OnTriggeredEvent(EventConfig eventConfig)
    {
        if(eventConfig == null) return;

        if (eventConfig is MusicPlayerEvent playerEvent)
        {
            await Task.Delay(playerEvent.StartDelayDuration.ToMilliseconds());
            PlayMusic(playerEvent.MusicGroup);
        }
    }

    private void PlayNextSong(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StopMusic();
        playedMusic.Push(currentMusic);
        PlayMusic(CurrentMusicGroup.GetSong());
    }

    private void PlayePreviousSong(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StopMusic();

        var previousMusicResults = GetPreviousMusic();

        if(!string.IsNullOrWhiteSpace(previousMusicResults.errorMessage))
        {
            Debug.LogError(previousMusicResults.errorMessage);
            return;
        }

        PlayMusic(previousMusicResults.music);
    }

    private void Initialize()
        => PlayMusic(currentGroup);

    private void OnSongStartedEvent(AudioType player, AudioConfig config)
    {
        if (player != AudioType.Music) return;

        if (config is MusicConfig music)
        {
            if (playedMusic.Count == 0)
                playedMusic.Push(music);

            currentMusic = music;
            onSongStartedEvent?.Invoke(music);
        }
    }

    private void OnSongCompletedEvent(AudioType player, AudioConfig config)
    {
        if (player != AudioType.Music) return;

        if (config is MusicConfig music)
        {
            playedMusic.Push(music);
            AudioManager.Instance.PlayAudio(CurrentMusicGroup.GetSong());
        }
    }

    private (MusicConfig music, string errorMessage) GetPreviousMusic()
    {
        if(playedMusic.Count > 0)
            previousMusic = playedMusic.Pop();

        if(previousMusic == null)
            previousMusic = CurrentMusicGroup.Music.FirstOrDefault();

        string errorMessage = previousMusic == null? $"{nameof(previousMusic)} cannot be null." : string.Empty;
        return (previousMusic, errorMessage);
    }

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
        PlayMusic(CurrentMusicGroup.GetSong());
    }

    public void PlayMusic(MusicConfig Config)
        => AudioManager.Instance.PlayAudio(Config);

    public void StopMusic()
        => AudioManager.Instance.StopAudio(AudioType.Music);

    public (MusicGroup group, string errorMessage) GetMusicGroup(MusicGroupType groupType)
    {
        MusicGroup group = musicGroups.FirstOrDefault(x => x.GroupType == groupType);
        string errorMessage = group == null ? $"Couldn't find music group of type: {groupType}" : string.Empty;
        return (group, errorMessage);
    }
}
