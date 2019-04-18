﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeController : MonoBehaviour
{
    public SFXController music;
    public AudioSource audio;
    public Slider slider;

    public void Awake()
    {
        music = GameObject.FindObjectOfType<SFXController>();
        audio = music.GetComponent<AudioSource>();

        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        audio.volume = slider.value;
        Debug.Log(slider.value);
    }
}