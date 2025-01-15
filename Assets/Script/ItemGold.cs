using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGold : ItemBase
{
    [SerializeField] private Transform target;   // Ÿ��
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private PhotonView photonView;

    public float speed = 5f;   // ���� �ӵ�

    public float moveAwayDistance = 2f; // �־��� �Ÿ�
    public float duration = 1f; // �־����� �� �ɸ��� �ð�

    private bool isMovingAway = false; // ���� �־����� ������ ����
    private bool firstMove = false;

    private void Update()
    {
        if (target == null) return;

        // Ÿ�� �ݴ� �������� �̵� ����
        Vector3 directionAway = (transform.position - target.position).normalized;
        Vector3 targetPosition = transform.position + directionAway * moveAwayDistance;

        if (!firstMove) StartCoroutine(MoveAwaySmoothly(targetPosition));

        // ���� ����
        if (!isMovingAway)
        {
            // ���� ������Ʈ�� Ÿ�� ������Ʈ �������� �̵�
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) < 0.3f) photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        }
    }

    public override void Use(GameObject entity)
    {
        target = entity.transform.GetChild(0).transform;
    }

    [PunRPC]
    public override void ItemSetUp(int callerViewID, string weaponBaseJson)
    {

    }

    private void OnEnable()
    {

    }

    [PunRPC]
    public override void PickUp(int index, int callerViewID)
    {

    }

    private IEnumerator MoveAwaySmoothly(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        firstMove = true;
        isMovingAway = true;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // ���� ���� (0���� 1����)

            // �ε巯�� �̵� (Lerp ���)
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null; // ���� �����ӱ��� ���
        }

        // ��Ȯ�� ��ǥ ��ġ�� ����
        transform.position = targetPosition;
        isMovingAway = false;
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
