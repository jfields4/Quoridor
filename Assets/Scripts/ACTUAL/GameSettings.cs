using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public bool AIGame;

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
}
