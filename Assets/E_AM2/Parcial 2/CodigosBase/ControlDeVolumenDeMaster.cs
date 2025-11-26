using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ControlDeVolumenDeMaster : MonoBehaviour
{
    public AudioMixer mixer;
    public string volumeParam = "MasterVol";

    public void SetVolume01(float value01)
    {
        value01 = Mathf.Clamp01(value01);

        float dB = (value01 > 0.0001f) ? Mathf.Log10(value01) * 20f : -80f;

        mixer.SetFloat(volumeParam, dB);
    }
}
