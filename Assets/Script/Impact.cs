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
        if (!particle.IsAlive(true)) // ��ƼŬ�� ������ ����Ǿ����� Ȯ��
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
    /// ��� Ŭ���̾�Ʈ���� ������Ʈ ��Ȱ��ȭ
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
