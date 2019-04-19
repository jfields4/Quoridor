using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float leftRight;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftRight = -50;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            leftRight = 50;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftRight = 0;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            leftRight = 0;
        }

        transform.Rotate(0, leftRight * Time.deltaTime, 0);
    }
}