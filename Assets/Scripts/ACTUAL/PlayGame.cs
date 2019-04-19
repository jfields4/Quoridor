using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayGame : MonoBehaviour
{

    public Button btPlay1, btPlay2;
    public GameSettings set;

    void Start() {
        btPlay1.onClick.AddListener(GoToGameOptions);
        btPlay2.onClick.AddListener(GoToLobby);

        set = GameObject.FindObjectOfType<GameSettings>();
    }

    public void GoToGameOptions() {
        SceneManager.LoadScene(2);
        set.AIGame = true;
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(7);
        set.AIGame = false;
    }
}
