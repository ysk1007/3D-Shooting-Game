using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    [SerializeField] private List<string> playerNickNames = new List<string>();
    [SerializeField] private List<TextMeshProUGUI> playerNickNameTexts = new List<TextMeshProUGUI>();

    [SerializeField] private GameObject player;

    [Space]
    [SerializeField] private Transform spawnPosition;

    [Space]
    [SerializeField] private GameObject poolSet;

    [Space]
    [SerializeField] private GameObject roomCam;

    [Space]
    [SerializeField] private GameObject roomUi;
    [SerializeField] private GameObject connectingUi;

    [SerializeField] private string nickname = "unnamed";
    [SerializeField] private string roomNameToJoin = "test";

    int playerCount = 0;
    public string RoomNameToJoin { 
        get => roomNameToJoin;
        set => roomNameToJoin = value; 
    }

    private void Awake()
    {
        instance = this;
        PhotonNetwork.SendRate = 30;         // �ʴ� �۽� Ƚ��
        PhotonNetwork.SerializationRate = 30; // �ʴ� ����ȭ Ƚ��
        PhotonNetwork.ConnectToRegion("kr"); // �ѱ� ����
    }

    public void ChangeRoomName(string name)
    {
        nickname = name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("������ ���� ��...");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // �ִ� 4���� �÷��̾� ���
            IsVisible = true, // �ٸ� �÷��̾�� ����
            IsOpen = true // �뿡 ���� ���� ����
        };

        roomNameToJoin = nickname + "���� ��";

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, roomOptions, TypedLobby.Default);

        //roomUi.SetActive(false);
        //connectingUi.SetActive(true);
    }

/*    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("������ ���� ��");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("�κ� ���� ��");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // �ִ� 4���� �÷��̾� ���
            IsVisible = true, // �ٸ� �÷��̾�� ����
            IsOpen = true // �뿡 ���� ���� ����
        };

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, roomOptions, TypedLobby.Default);

    }*/

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("�뿡 ���� ��");
        playerCount++;
        //roomCam.SetActive(false);

        //RespawnPlayer();

        //poolSet.SetActive(true);
        //ItemMemoryPool.instance.TestSpawn();

        for (int i = 0; i < playerCount; i++)
        {
            playerNickNameTexts[i].text = nickname;
        }
    }

    public void RespawnPlayer()
    {
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPosition.position, Quaternion.identity);
        _player.GetComponent<PlayerSetUp>().InLocalPlayer();
        _player.GetComponent<PhotonView>().RPC("SetNickName",RpcTarget.AllBuffered, nickname);
    }
}
