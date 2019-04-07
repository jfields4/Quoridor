using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenNavigator : MonoBehaviour
{
    public Button mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        if(mainMenu)
        {
            mainMenu.onClick.AddListener(GoToMainMenu);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
