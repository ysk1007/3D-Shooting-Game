using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject PlayerListItemPrefab;
    [SerializeField] private GameObject startGameButton;

    [SerializeField] private List<GameObject> playerItemList;

    [SerializeField] private GameObject roomScreen;
    [SerializeField] private GameObject lobbyScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void changedNickName(string s)
    {
        PhotonNetwork.NickName = s;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        //MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text + "¥‘¿« πÊ");
        //MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        //MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        PlayerItemUpdate();

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        roomScreen.SetActive(false);
        lobbyScreen.SetActive(true);
        //MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        roomScreen.SetActive(true);
        lobbyScreen.SetActive(false);
        //MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        //MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomItem>().SetUp(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        PlayerItemUpdate();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PlayerItemUpdate();
    }

    public void PlayerItemUpdate()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        foreach (GameObject item in playerItemList)
        {
            item.SetActive(false);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            playerItemList[i].SetActive(true);
            playerItemList[i].GetComponent<PlayerListItem>().SetUp(players[i]);
        }
    }
}