using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public bool AIGame;
    public bool AIHard;

    private static GameSettings instance = null;
    public static GameSettings Instance
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
    }

    public void SetToAIGame()
    {
        AIGame = true;
    }

    public void SetToNetworkGame()
    {
        AIGame = false;
    }

    public void SetAIToHard()
    {
        AIHard = true;
    }

    public void SetAIToEasy()
    {
        AIHard = false;
    }
}
