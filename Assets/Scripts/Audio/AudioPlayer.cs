using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioType player;

    private AudioSource playerAudioSource;

    public AudioSource PlayerAudioSource
    {
        get
        {
            if (playerAudioSource == null)
            {
                playerAudioSource = GetComponent<AudioSource>();

                if(playerAudioSource == null)
                    playerAudioSource = gameObject.AddComponent<AudioSource>();
            }
            return playerAudioSource;
        }
    }

    public AudioType Player => player;

    public async void Play(AudioConfig config)
    {
        PlayerAudioSource.clip = config.Clip;
        PlayerAudioSource.loop = config.Loop;

        int delay = (int)(config.Delay * 1000);
        await Task.Delay(delay);

        playerAudioSource.Play();

        AudioManager.Instance.OnPlayerStartedEvent?.Invoke(Player, config);

        while(PlayerAudioSource.isPlaying)
            await Task.Yield();

        AudioManager.Instance.OnPlayerCompletedEvent?.Invoke(Player, config);

    }
}
