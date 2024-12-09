using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Color32[] textColors;
    [SerializeField] private Animator textAnimator;
    MemoryPool memoryPool;
    public void SetUp(MemoryPool memoryPool, float damage, bool critical)
    {
        this.memoryPool = memoryPool;
        damageText.text = damage.ToString("F0");
        damageText.color = critical ? textColors[1] : textColors[0];
        textAnimator.SetTrigger("Show");
    }

    public void Destroy()
    {
        // 텍스트 제거
        memoryPool.DeactivatePoolItem(this.gameObject);
    }

}
