using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject poolSet;

    [SerializeField] private GameObject roomCam;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Debug.Log("������ ���� ��...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
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

        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("�뿡 ���� ��");

        roomCam.SetActive(false);

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPosition.position, Quaternion.identity);
        _player.GetComponent<PlayerSetUp>().InLocalPlayer();


        poolSet.SetActive(true);
        //ItemMemoryPool.instance.TestSpawn();
    }

    public void RespawnPlayer()
    {
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPosition.position, Quaternion.identity);
        _player.GetComponent<PlayerSetUp>().InLocalPlayer();
    }
}
