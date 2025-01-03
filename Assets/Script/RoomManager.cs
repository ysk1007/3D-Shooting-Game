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
        Debug.Log("서버에 연결 중...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("서버에 연결 됨");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("로비에 연결 됨");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // 최대 4명의 플레이어 허용
            IsVisible = true, // 다른 플레이어에게 공개
            IsOpen = true // 룸에 참가 가능 여부
        };

        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("룸에 연결 됨");

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
