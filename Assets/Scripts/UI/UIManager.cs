using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject RoomPrefab;
    private ulong roomSelected = (ulong)NetworkID.Invalid;
    private int roomsCreated = 0;
    private List<Room> roomsAvailable;
    /// <summary>
    /// Awake method transforms this class into a singleton.
    /// To access this singleton from another class, see the public variable:
    void Awake()
    {
        // Performing a Singleton.
        if (UIManager.Instance != null)
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
    /// <summary>
    /// A structure to hold information regarding a room.
    /// </summary>
    private class Room
    {
        public string name;
        public GameObject instance;
        public NetworkID networkId;
        /// <summary>
        /// Constructor for the RoomInformation.
        /// </summary>
        /// <param name="Name"> The name of the room. </param>
        /// <param name="Instance"> The GameObject that represents the room on the list. </param>
        public Room(string Name, GameObject Instance, NetworkID id)
        {
            name = Name;
            instance = Instance;
            networkId = id;
        }
    }
    // Use this for initialization
    void Start()
    {
        roomsAvailable = new List<Room>();
        // Start the matchmaking
        LobbyManager.Instance.StartMatchMaker();
        StopAllCoroutines();
        StartCoroutine(RefreshLobby());
    }
    // Update is called once per frame
    void Update() { }
    /// <summary>
    /// Fill the list of matches on the UI.
    /// </summary>
    /// <param name="matchList">The list of matches available online.</param>
    public void ListMatches(List<MatchInfoSnapshot> matchList)
    {
    }
    /// <summary>
    /// Creates a room and waits for player.
    /// </summary>
    public void OnClickCreateRoomButton()
    {
        /* Creates the match. */
        LobbyManager.Instance.matchMaker.CreateMatch(
            "New Room",
            (uint)LobbyManager.Instance.maxPlayers,
            true,
            "", "", "", 0, 0,
            LobbyManager.Instance.OnMatchCreate);
    }
    /// <summary>
    /// When the player click to join the room.
    /// </summary>
    public void OnClickJoinRoomButton()
    {
        if (roomSelected == (ulong)NetworkID.Invalid)
        {
            Debug.Log("<color=orange>UIManager:</color> There seems to be an error the selected room doesn't exist.");
            return;
        }
        NetworkID selected = roomsAvailable.Find(R => R.networkId == (NetworkID)roomSelected).networkId;
        /* Call the method that tries to login */
        Debug.Log("Attempt to join.");
        LobbyManager.Instance.matchMaker.JoinMatch(selected, "", "", "", 0, 0, LobbyManager.Instance.OnMatchJoined);
    }

    /// <summary>
    /// Creates a room on the user interface.
    /// </summary>
    public void CreateRoomOnUI(NetworkID networkID)
    {
        GameObject parent = GameObject.Find("MatchesPanel/Container");
        /* Creates the room and set it under the container object.*/
        GameObject room = GameObject.Instantiate(RoomPrefab, parent.transform);
        roomsCreated++;
        room.GetComponentInChildren<Text>().text = "Room " + roomsCreated;
        room.GetComponent<Button>().onClick.RemoveAllListeners();
        room.GetComponent<Button>().onClick.AddListener(delegate { ChangeSelectedRoom(networkID); });
        roomsAvailable.Add(new Room("Room " + roomsCreated, room, networkID));
    }
    /// <summary>
    /// Creates a room on the user interface.
    /// </summary>
    public void RemoveRoomOnUI(NetworkID networkID)
    {
        Room elementToBeDeleted = roomsAvailable.Find(R => R.networkId == networkID);
        if (elementToBeDeleted != null)
        {
            roomsAvailable.Remove(elementToBeDeleted);
            GameObject.Destroy(elementToBeDeleted.instance.gameObject);
        }
        else
            Debug.LogError("<color=orange>UIManager:</color>Could not find the element to destroy.");
    }
    public void UpdateMatchList(List<MatchInfoSnapshot> matchList)
    {
        /* Case there is no match. */
        if (matchList.Count == 0)
        {
            int elementIndex = 0;
            while (roomsAvailable.Count != 0)
            {
                RemoveRoomOnUI(roomsAvailable[elementIndex].networkId);
            }
            return;
        }
        /* Check every room online to see if it exist on the UI if not create it.*/
        for (int i = 0; i < matchList.Count; ++i)
        {
            /* If room exist online but not on UI create it.*/
            if (roomsAvailable.Find(R => R.networkId == matchList[i].networkId) == null)
                CreateRoomOnUI(matchList[i].networkId);
        }
        /* Check every room on the UI to see if it exists online.*/
        List<Room> roomListCopy = new List<Room>(roomsAvailable);
        foreach (Room roomy in roomListCopy)
        {
            MatchInfoSnapshot matchInfoSnapshot = matchList.Find(N => N.networkId == roomy.networkId);
            /* If doesn't exist remove from UI. */
            if (matchInfoSnapshot == null)
                RemoveRoomOnUI(roomy.networkId);
        }
    }
    /// <summary>
    /// Change the selected room on the UI
    /// </summary>
    /// <param name="selection"></param>
    public void ChangeSelectedRoom(NetworkID selection)
    {
        this.roomSelected = (ulong)selection;
    }
    /// <summary>
    /// Method that refreshes the lobby interface.
    /// </summary>
    private void RefreshMatchList()
    {
        // Set the callback to do something once the matchmaker have started
        if (LobbyManager.Instance == null)
        {
            Debug.Log("Lobby manager seems to be destroyed..");
            return;
        }
        if (LobbyManager.Instance.matchMaker == null)
        {
            LobbyManager.Instance.StartMatchMaker();
        }
        LobbyManager.Instance.matchMaker.ListMatches(0, 6, "", true, 0, 0, LobbyManager.Instance.OnMatchList);
    }
    /// <summary>
    /// Coroutine that calls RefreshMatchList every 1 second.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshLobby()
    {
        while (true)
        {
            RefreshMatchList();
            yield return new WaitForSeconds(1f);
        }
    }
}
