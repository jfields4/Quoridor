using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    private static SFXController instance = null;
    public AudioSource audio;
    public AudioClip wall;

    public static SFXController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        audio = GetComponent<AudioSource>();
    }

    public void WallSound()
    {
        audio.clip = wall;
        audio.Play();
    }
}