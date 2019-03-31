using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    public GameObject music;
    public AudioSource audio;
    public Slider slider;

    public void Awake()
    {
        music = GameObject.Find("Music");
        Debug.Log("Music check: " + music);

        audio = music.GetComponent<AudioSource>();
        Debug.Log("Audio check: " + audio);

        slider = GetComponent<Slider>();
        Debug.Log("Slider check: " + slider);
    }

    public void OnValueChanged()
    {
        audio.volume = slider.value;
        Debug.Log(slider.value);
    }
}
