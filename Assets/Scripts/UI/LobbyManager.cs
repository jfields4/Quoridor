using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
/// <summary>
/// Class that hold the Lobby methods for creating and managing rooms. 
/// </summary>
public class LobbyManager : NetworkLobbyManager
{
    #region Properties
    /// <summary>
    /// Variable to save memory address reference to this singleton.
    /// </summary>
    public static LobbyManager Instance;
    public MatchInfo CurrentMatch;
    #endregion
    #region Attributes
    private UIManager uiManagerReference;
    #endregion
    #region Methods
    #region Unity Methods
    // ------------------------------------------------------------------------
    /// <summary>
    /// Awake method transforms this class into a singleton.
    /// To access this singleton from another class, see the public variable:
    /// <see cref="Instance"/>
    /// </summary>
    void Awake()
    {
        // Performing a Singleton.
        if (LobbyManager.Instance != null)
        {
            Destroy(this.gameObject);
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    // ------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        if (uiManagerReference == null)
            uiManagerReference = GameObject.Find("Managers/UI Manager").GetComponent<UIManager>();
        // Start the matchmaking
        LobbyManager.Instance.StartMatchMaker();
    }
    // ------------------------------------------------------------------------
    #endregion
    #region Public Methods
    // ------------------------------------------------------------------------
    /// <summary>
    /// Callback that is called whenever the matches are listed from the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description of the failure if success is false.</param>
    /// <param name="matchList">Information about the match created.</param>
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        uiManagerReference.UpdateMatchList(matchList);
    }
    // ------------------------------------------------------------------------
    /// <summary>
    /// Event that runs when a match is created.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description of the failure if success is false.</param>
    /// <param name="matchInfo">Information about the match created.</param>
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        /* If there is no reference to the UI Manager */
        if (uiManagerReference == null)
            uiManagerReference = GameObject.Find("Managers/UI Manager").GetComponent<UIManager>();
        /* Check if recovered. */
        if (uiManagerReference == null)
            Debug.Log("<color=red>Lobby Manager:</color> Can't find the local area UI. Are you sure this is being called on the correct scene?");
        else
            uiManagerReference.CreateRoomOnUI(matchInfo.networkId);
        /* Save a reference to the current match.. (server only)*/
        CurrentMatch = matchInfo;
    }
    #endregion
    #endregion
}