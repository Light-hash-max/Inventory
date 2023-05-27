using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolume : MonoBehaviour
{
    enum State { Min, Max}

    [SerializeField] AudioSource Audio;
    [Range (0f, 1f)]
    [SerializeField] float MinVolume;
    [Range (0f, 1f)]
    [SerializeField] float MaxVolume;
    [SerializeField] float TimeToChange = 0.2f;

    State VolumeState;
    float Step;

    private void Start ()
    {
        if (Audio.volume == MinVolume) VolumeState = State.Min;
        else VolumeState = State.Max;

        Step = (MaxVolume - MinVolume) / TimeToChange;
    }

    private void Update ()
    {
        switch (VolumeState) {
            case State.Min:
                if (Audio.volume != MinVolume) {
                    Audio.volume -= Step * Time.deltaTime;
                    if (Audio.volume < MinVolume) Audio.volume = MinVolume;
                }
                break;

            case State.Max:
                if (Audio.volume != MaxVolume) {
                    Audio.volume += Step * Time.deltaTime;
                    if (Audio.volume > MaxVolume) Audio.volume = MaxVolume;
                }
                break;
        }
    }

    public void SetMax ()
    {
        VolumeState = State.Max;
    }

    public void SetMin ()
    {
        VolumeState = State.Min;
    }

    public void Change ()
    {
        if (VolumeState == State.Max) VolumeState = State.Min;
        else VolumeState = State.Max;
    }
}
