using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashFlow : MonoBehaviour
{
    private const int Index = 1;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("Countdown");
    }

    private IEnumerator Countdown() 
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }

    public void NextScene() {
        Debug.Log("next scene");
        SceneManager.LoadScene(1);
    }
}
