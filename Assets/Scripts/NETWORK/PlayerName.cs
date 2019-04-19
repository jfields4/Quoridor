using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//working off of this tutorial, will adjust as needed:
//https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/pun-basics-tutorial/lobby-ui

public class PlayerName : MonoBehaviour
{
    string playerName;

    void Start()
    {
        InputField inputField = this.GetComponent<InputField>();
    }

    public void SetPlayerName(string name)
    {
        if(name == null || name == "")
        {
            Debug.Log("No name entered");
            return;
        }

        PhotonNetwork.NickName = name;
    }
}
