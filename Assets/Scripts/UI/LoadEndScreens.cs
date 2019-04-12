using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadEndScreens : MonoBehaviour
{
    public void ShowWinScreen()
    {
        SceneManager.LoadScene(8);
    }

    public void ShowLossScreen()
    {
        SceneManager.LoadScene(9);
    }
}
