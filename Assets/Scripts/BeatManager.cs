using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Beats[] _beats;
    [SerializeField] private AudioMixerGroup pitchBendGroup;

    private void Update()
    {
        foreach (Beats beat in _beats)
        {
            float sampledTime = _audioSource.timeSamples / (_audioSource.clip.frequency * beat.GetBeatLength(_bpm));
            beat.CheckForNewBeat(sampledTime);
        }
    }

    public void UpdateBPM(float tempoIncrease)
    {
        _audioSource.outputAudioMixerGroup = pitchBendGroup;

        _audioSource.pitch *= tempoIncrease;
        pitchBendGroup.audioMixer.SetFloat("PitchBend", 1f / tempoIncrease);

        // // make audio faster (use pitch)
        // _audioSource.pitch *= tempoIncrease;

        // update beatTempo to match new audio tempo
        _bpm *= tempoIncrease;
    }
}

[System.Serializable]
public class Beats
{
    // steps allows us to have quarter, half, etc beat
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int _lastBeat;

    public float GetBeatLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewBeat(float beat)
    {
        if (Mathf.FloorToInt(beat) != _lastBeat)
        {
            // we want to check every whole number - when the number passes a whole number, we've passed a new beat
            // we use FloorToInt because we'll rarely have a whole number
            _lastBeat = Mathf.FloorToInt(beat);
            _trigger.Invoke();
        }
    }
}