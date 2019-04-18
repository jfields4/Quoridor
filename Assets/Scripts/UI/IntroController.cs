using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void Start()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    void Update()
    {
        if (Input.anyKey)
        {
            EndReached(videoPlayer);
        }
    }

    public void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        vp.Stop();
        SceneManager.LoadScene(1);
    }
}
