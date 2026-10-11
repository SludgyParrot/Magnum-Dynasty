using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

public sealed class AudioManager : SingletonInstance<AudioManager>
{
    [SerializeField]
    private List<AudioPlayer> audioPlayers = 
        new List<AudioPlayer>();

    [field: SerializeField, Space(5)]
    public UnityEvent<AudioType, AudioConfig> OnPlayerStartedEvent { get; private set; }

    [field: SerializeField, Space(5)]
    public UnityEvent<AudioType, AudioConfig> OnPlayerCompletedEvent { get; private set; }

    public void PlayAudio(AudioConfig config)
    {
        switch(config)
        {
            case MusicConfig music:
                PlayAudio(AudioType.Music, config);
                break;
        }
    }

    public void PlayAudio(AudioType player, AudioConfig config)
    {
        var playerResults = GetAudioPlayer(player);

        if(!string.IsNullOrWhiteSpace(playerResults.errorMessage))
        {
            Debug.LogError(playerResults.errorMessage);
            return;
        }

        playerResults.player.Play(config);
    }

    public void StopAudio()
    {
        foreach (var player in audioPlayers)
            StopAudio(player.Player);
    }

    public void StopAudio(AudioType player)
    {
        var playerResults = GetAudioPlayer(player);

        if (!string.IsNullOrWhiteSpace(playerResults.errorMessage))
        {
            Debug.LogError(playerResults.errorMessage);
            return;
        }

        playerResults.player.Stop();
    }

    public (AudioPlayer player, string errorMessage) GetAudioPlayer(AudioType player)
    {
        AudioPlayer audioPlayer = audioPlayers.FirstOrDefault(x => x.Player == player);
        string errorMessage = audioPlayer == null ? $"Couldn't find audio player of type: {player}" : string.Empty;
        return (audioPlayer, errorMessage);
    }
}
