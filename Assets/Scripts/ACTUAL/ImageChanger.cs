using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    public Sprite[] TutorialImages;
    public int CurrentTutorialImage;
    public GameObject CurrentTutorialImageGameobject;
    // Start is called before the first frame update
    void Start()
    {
        CurrentTutorialImage = 0; 
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Left()
    {
        if(CurrentTutorialImage>0)
        {
            CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[CurrentTutorialImage - 1];
            CurrentTutorialImage--;
        }
    }

    public void Right()
    {
        if (CurrentTutorialImage < 3)
        {
            CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[CurrentTutorialImage + 1];
            CurrentTutorialImage++;
        }
    }

    public void HowToPlay()
    {
        CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[0];
        CurrentTutorialImage = 0;
    }

    public void MovePlayer()
    {
        CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[1];
        CurrentTutorialImage = 1;
    }

    public void PlaceWalls()
    {
        CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[2];
        CurrentTutorialImage = 2;
    }

    public void CameraMovement()
    {
        CurrentTutorialImageGameobject.gameObject.GetComponent<Image>().sprite = TutorialImages[3];
        CurrentTutorialImage = 3;
    }
}
