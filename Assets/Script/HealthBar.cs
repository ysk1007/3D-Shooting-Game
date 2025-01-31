using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviourPun
{
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider easeHpSlider;

    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float hpValue;

    [SerializeField]
    private float lerpSpeed = 0.05f;

    public PhotonView photonView;


    public void Setup(float maxHP)
    {
        maxValue = maxHP;
        hpValue = maxValue;
        hpSlider.maxValue = maxValue;
        easeHpSlider.maxValue = maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (hpSlider.value != hpValue)
        {
            hpSlider.value = hpValue;
        }

        if (hpSlider.value != easeHpSlider.value)
        {
            easeHpSlider.value = Mathf.Lerp(easeHpSlider.value, hpValue, lerpSpeed);
        }
        //photonView.RPC("SliderUpdate", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void takeDamage(float damage)
    {
        hpValue = hpValue - damage >= maxValue ? maxValue : hpValue - damage;
    }

    // RPC를 통해 네트워크에서 동기화
    [PunRPC]
    void SliderUpdate()
    {
        if (hpSlider.value != hpValue)
        {
            hpSlider.value = hpValue;
        }

        if (hpSlider.value != easeHpSlider.value)
        {
            easeHpSlider.value = Mathf.Lerp(easeHpSlider.value, hpValue, lerpSpeed);
        }
    }
}
