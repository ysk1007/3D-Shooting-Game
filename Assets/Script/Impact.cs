using Photon.Pun;
using System.Buffers;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(PhotonView))]
public class Impact : MonoBehaviourPunCallbacks
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void SetUp(MemoryPool pool)
    {
        memoryPool = pool;
        photonView.RPC(nameof(ActivateObjectRPC), RpcTarget.AllBuffered, true);
    }

    private void Update()
    {
        if (!particle.IsAlive(true)) // 파티클이 완전히 종료되었는지 확인
        {
            DeactivateImpact();
        }
    }

    private void DeactivateImpact()
    {
        memoryPool?.DeactivatePoolItem(gameObject);
        photonView.RPC(nameof(ActivateObjectRPC), RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 모든 클라이언트에서 오브젝트 비활성화
    /// </summary>
    [PunRPC]
    public void DeactivateObjectRPC(int viewID)
    {
        memoryPool.ActiveCount--;
        PhotonView targetPhotonView = PhotonNetwork.GetPhotonView(viewID);

        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[MemoryPool] DeactivateObjectRPC: PhotonView with ID {viewID} not found.");
        }
    }
}
