using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<float, float> { }
public class Status : MonoBehaviourPun
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    private float walkSpeedTemp;

    [SerializeField]
    private float runSpeed;
    private float runSpeedTemp;

    [Header("HP")]
    [SerializeField]
    private float maxHP = 100;
    [SerializeField]
    private float currentHP;

    [Header("Attack")]
    [SerializeField]
    private float damage;

    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float RunSpeed { get => runSpeed; set => runSpeed = value; }

public float CurrentHP => currentHP;
    public float MaxHP => maxHP;

    public float Damage => damage;

    private void Awake()
    {
        currentHP = maxHP;
        runSpeedTemp = runSpeed;
        walkSpeedTemp = walkSpeed;
    }

    private void OnEnable()
    {
        currentHP = maxHP;
        runSpeedTemp = runSpeed;
        walkSpeedTemp = walkSpeed;
    }

    [PunRPC]
    public void DecreaseHP(float damage)
    {
        float previousHP = currentHP;

        currentHP = CurrentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(previousHP, currentHP);
    }

    [PunRPC]
    public void IncreaseHp(float hp)
    {
        float previousHP = currentHP;

        currentHP = CurrentHP + hp > maxHP ? maxHP : currentHP + hp;

        onHPEvent.Invoke(previousHP, currentHP);
    }

    public bool isDie()
    {
        if (currentHP <= 0)
        {
            GetComponent<EnemyFSM>().photonView.RPC("Die",RpcTarget.AllBuffered);
            return true;
        }

        return false;
    }
}
