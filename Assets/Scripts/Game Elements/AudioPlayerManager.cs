using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerManager : MonoBehaviour
{
    // Pool audio players.
    [SerializeField] private AudioSource _audioPlayerPrefab;
    [SerializeField] private int _objectPoolCount;      // Initial number of objects to create.

    private List<AudioSource> _audioPlayers = new List<AudioSource>();

    public static AudioPlayerManager Instance = null;

    private void Awake()
    {
        Instance = this;
        _audioPlayers = new List<AudioSource>(_objectPoolCount);
        for (int i = 0; i < _objectPoolCount; i++)
        {
            CreateAudioPlayer();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void PlayAudio(AudioClip clipToPlay)
    {
        AudioSource audioPlayer = GetAvailableAudioPlayer();
        audioPlayer.clip = clipToPlay;
        audioPlayer.Play();
    }

    private AudioSource CreateAudioPlayer()
    {
        AudioSource newAudioPlayer = Instantiate(_audioPlayerPrefab);
        _audioPlayers.Add(newAudioPlayer);
        return newAudioPlayer;
    }

    private AudioSource GetAvailableAudioPlayer()
    {
        AudioSource freeAudioPlayer = _audioPlayers.Find(x => !x.isPlaying);
        if (freeAudioPlayer) return freeAudioPlayer;
        else return CreateAudioPlayer();
    }
}
