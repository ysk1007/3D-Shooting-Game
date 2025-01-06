using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private string roomName;
    
    public string RoomName
    {
        get => roomName;
        set => roomName = value;
    }

    public void OnButtonPressed()
    {
        RoomList.instance.JoinRoomByName(roomName);
    }
}
