using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainNavigator : MonoBehaviour
{

    public Button bt_play, bt_back;
    void Start()
    {
        if (bt_play) bt_play.onClick.AddListener(GoToFullGame);
        if (bt_back) bt_back.onClick.AddListener(GoToMainMenu);
    }

    public void GoToFullGame()
    {
        SceneManager.LoadScene(4);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
