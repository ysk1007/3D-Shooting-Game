using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [SerializeField] private GameObject roomScreen;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)  // 인게임 씬에 있음
        {
            roomScreen.SetActive(false);
            GameObject _player = PhotonNetwork.Instantiate(Path.Combine("PlayerArmature"), Vector3.zero, Quaternion.identity);
            _player.GetComponent<PlayerSetUp>().InLocalPlayer();
            _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
        }
    }
}
