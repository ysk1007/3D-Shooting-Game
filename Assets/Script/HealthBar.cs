using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
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

    public void Setup(float maxHP)
    {
        maxValue = maxHP;
        hpValue = maxValue;
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
    }

    public void takeDamage(float damage)
    {
        hpValue -= damage;
    }
}
