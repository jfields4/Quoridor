using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MenuMusicController;

public class MainMenu : MonoBehaviour
{
    public Button play, options, quit, howto;

    void Start()
    {
        quit.onClick.AddListener(ExitGame);
        play.onClick.AddListener(GoToPlayGame);
        options.onClick.AddListener(GoToGlobalOptions);
        howto.onClick.AddListener(GoToHowTo);
    }

    public void GoToGlobalOptions()
    {
        SceneManager.LoadScene(6);
    }

    public void GoToHowTo()
    {
        SceneManager.LoadScene(5);
    }

    public void GoToPlayGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame() {
        Debug.Log("Exit");
        Application.Quit();
    }
}
