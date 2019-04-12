using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EndScreenManager : MonoBehaviour
{
    public Button mainMenuButton;

    void OnEnable()
    {
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
