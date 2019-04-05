using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicVolumeController : MonoBehaviour
{
    public MusicController music;
    public AudioSource audio;
    public Slider slider;

    public void Awake()
    {
        music = GameObject.FindObjectOfType<MusicController>();
        Debug.Log(music);
        audio = music.GetComponent<AudioSource>();
        Debug.Log(audio);
        Debug.Log(slider);

        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        Debug.Log("Slider value: " + slider.value);
        Debug.Log("Original volume: " + slider.value);
        audio.volume = slider.value;
        Debug.Log("New volume: " + audio.volume);
    }
}
