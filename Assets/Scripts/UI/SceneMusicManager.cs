using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour
{
    public bool inMenu = true;
    public AudioClip menu;
    public AudioClip game;
    public MusicController music;
    public AudioSource audio;

    // Start is called before the first frame update
    void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //music = GameObject.FindObjectOfType<MusicController>();
        audio = music.GetComponent<AudioSource>();
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        Debug.Log(sceneName);
        if (sceneName == "Game")
        {
            Debug.Log("Changing music to " + game);
            audio.clip = game;
            audio.Play();
            inMenu = false;
        }
        else if (!inMenu)
        {
            Debug.Log("Changing music to " + menu);
            audio.clip = menu;
            audio.Play();
            inMenu = true;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
