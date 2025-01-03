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
        // ��ƼŬ�� ������� �ƴϸ� ����
        if (particle.isPlaying == false)
        {
            // ����Ʈ ������Ʈ ����
            memoryPool?.DeactivatePoolItem(this.gameObject);
            photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        }
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
