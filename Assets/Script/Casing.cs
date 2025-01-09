using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float deactivateTime = 5.0f;    // ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð�
    [SerializeField]
    private float casingSpin = 1.0f;        // ź�ǰ� ȸ���ϴ� �ӷ� ���
    [SerializeField]
    private AudioClip[] audioClips;         // ź�ǰ� �ε����� �� ����Ǵ� ����

    [SerializeField] private Rigidbody rigidbody3D;
    [SerializeField] private AudioSource audioSource;
    private MemoryPool memoryPool;
    private PhotonView photonView;

    public void Setup(MemoryPool pool, Vector3 direction)
    {
        photonView = GetComponent<PhotonView>();
        memoryPool = pool;

        // ź���� �̵� �ӷ°� ȸ�� �ӷ� ����
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin)
                                                  );

        // ź�� �ڵ� ��Ȱ��ȭ�� ���� �ڷ�ƾ ����
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ź�� ���� �� ������ ���� ����
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
