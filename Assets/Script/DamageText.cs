using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Color32[] textColors;
    [SerializeField] private Animator textAnimator;
    private PhotonView photonView;
    MemoryPool memoryPool;

    public void SetUp(MemoryPool memoryPool, float damage, bool critical)
    {
        photonView = GetComponent<PhotonView>();
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

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void DeactivateObjectRPC()
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(memoryPool.ParentTransform);
    }

}
