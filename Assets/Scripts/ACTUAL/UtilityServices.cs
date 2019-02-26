using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UtilityServices : MonoBehaviour
{


    public static UtilityServices instance { private set; get; }


    private void Awake()
    {
        instance = this;
    }



    /// <summary> Runs a command(function) specified in command argument after the time specified in the dalay argument has passed.</summary>

    public void RunDelayedCommand(float delay, Action command)
    {
        StartCoroutine(RunCommandAfter(delay, command));
    }




    /// <summary> Runs a command(function) specified in command argument after returning the specified IEnumerator object.</summary>

    public void RunDelayedCommand(Action delay, Action command)
    {
        StartCoroutine(RunCommandAfter(delay, command));
    }




    /// <summary> The coroutine that actually runs delayed command when a call to RunDelayedCommand function is made .</summary>

    private IEnumerator RunCommandAfter(float delay, Action command)
    {
        yield return new WaitForSeconds(delay);
        command();
    }



    /// <summary> The coroutine that actually runs delayed command when a call to RunDelayedCommand function with the IEnumerator is made.</summary>

    private IEnumerator RunCommandAfter(Action delay, Action command)
    {
        yield return delay;
        command();
    }


}
