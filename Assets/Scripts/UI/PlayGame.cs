﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{

    public Button btPlay1, btPlay2, btPlay3;

    void Start() {
        btPlay1.onClick.AddListener(GoToGameOptions);
        btPlay2.onClick.AddListener(GoToLobby);
    }

    public void GoToGameOptions() {
        SceneManager.LoadScene(2);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(7);
    }
}
