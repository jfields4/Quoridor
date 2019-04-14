using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public bool AIGame;
    public bool AIHard;

    void Awake()
    {
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
