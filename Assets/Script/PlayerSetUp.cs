using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSetUp : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private WeaponSwitchSystem weaponSwitchSystem;
    [SerializeField] private Status status;
    [SerializeField] private GameObject camera;
    [SerializeField] private string nickname;
    [SerializeField] private TextMeshProUGUI nicknameText;

    public void InLocalPlayer()
    {
        playerManager.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    void SetNickName(string name)
    {
        nickname = name;
        nicknameText.text = nickname;
    }
}
