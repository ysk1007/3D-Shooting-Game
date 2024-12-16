using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetUp : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private WeaponSwitchSystem weaponSwitchSystem;
    [SerializeField] private Status status;
    [SerializeField] private GameObject camera;

    public void InLocalPlayer()
    {
        playerManager.enabled = true;
        camera.SetActive(true);
    }

}
