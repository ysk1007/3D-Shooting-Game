using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private string roomName;

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCount;

    RoomInfo info;

    public string RoomName
    {
        get => roomName;
        set => roomName = value;
    }

    public void SetUp(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomNameText.text = roomInfo.Name;
        playerCount.text = roomInfo.PlayerCount.ToString() + "/4";
    }

    public void OnButtonPressed()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
