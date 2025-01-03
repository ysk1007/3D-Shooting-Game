using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviourPunCallbacks
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private MemoryPool memoryPool;
    [SerializeField] private PhotonView photonView;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        photonView = GetComponent<PhotonView>();
    }

    public void SetUp(MemoryPool pool)
    {
        memoryPool = pool;
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);
    }

    // Update is called once per frame
    void Update()
    {
        // 파티클이 재생중이 아니면 삭제
        if (particle.isPlaying == false)
        {
            // 임펙트 오브젝트 제거
            memoryPool?.DeactivatePoolItem(this.gameObject);
            photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        }
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
