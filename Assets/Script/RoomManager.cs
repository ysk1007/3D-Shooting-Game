using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject poolSet;

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

        PhotonNetwork.JoinOrCreateRoom("test",null,null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("룸에 연결 됨");

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPosition.position, Quaternion.identity);
        _player.GetComponent<PlayerSetUp>().InLocalPlayer();

        poolSet.SetActive(true);
        //ItemMemoryPool.instance.TestSpawn();
    }
}
