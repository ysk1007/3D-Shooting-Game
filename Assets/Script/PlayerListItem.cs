using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nicknameText;

    Photon.Realtime.Player player;

    public void SetUp(Photon.Realtime.Player player)
    {
        this.player = player;
        nicknameText.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        gameObject.SetActive(false);
    }
}
