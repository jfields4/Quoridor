using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

//Based on this tutorial, will adjust as needed:
//https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/lobby

public class Launcher : MonoBehaviourPunCallbacks
{
    private byte maxPlayers = 2;

    public GameObject Controls;
    public GameObject Connecting;
    public GameObject Waiting;

    public void Start()
    {
        Waiting.SetActive(false);
        Connecting.SetActive(false);
        Controls.SetActive(true);
    }

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Update()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
            {
                SceneManager.LoadScene(4);
            }
        }
    }

    public void Connect()
    {
        Connecting.SetActive(true);
        Controls.SetActive(false);
        Waiting.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string stringMessage)
    {
        Debug.Log("Could not join, creating room");
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayers});
        //TODO: "add waiting for player to join" text
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined");
        if (PhotonNetwork.IsMasterClient)
        {
            Waiting.SetActive(true);
            Connecting.SetActive(false);
            Controls.SetActive(false);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Waiting.SetActive(false);
        Connecting.SetActive(false);
        Controls.SetActive(true);
        Debug.Log("Disconnected");
    }
}
