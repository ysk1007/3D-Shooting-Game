using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExp: ItemBase
{
    [SerializeField] private Transform target;   // 타겟
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private PhotonView photonView;

    public float value = 5;      // 경험치
    public float speed = 5f;   // 추적 속도

    public float moveAwayDistance = 2f; // 멀어질 거리
    public float duration = 1f; // 멀어지는 데 걸리는 시간

    private bool isMovingAway = false; // 현재 멀어지는 중인지 여부
    private bool firstMove = false;

    ItemMemoryPool itemMemoryPool;

    private void Update()
    {
        if (target == null) return;

        // 타겟 반대 방향으로 이동 시작
        Vector3 directionAway = (transform.position - target.position).normalized;
        Vector3 targetPosition = transform.position + directionAway * moveAwayDistance;

        if (!firstMove) StartCoroutine(MoveAwaySmoothly(targetPosition));

        // 추적 로직
        if (!isMovingAway)
        {
            // 현재 오브젝트가 타겟 오브젝트 방향으로 이동
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
        PhotonView callerView = PhotonView.Find(callerViewID);
        this.itemMemoryPool = callerView.GetComponent<ItemMemoryPool>();

        photonView = GetComponent<PhotonView>();
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);
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
            float t = elapsedTime / duration; // 진행 비율 (0에서 1까지)

            // 부드러운 이동 (Lerp 사용)
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 정확히 목표 위치로 설정
        transform.position = targetPosition;
        isMovingAway = false;
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        GameManager.instance.UpdateExp(value);
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 모든 클라이언트에서 오브젝트 비활성화
    /// </summary>
    [PunRPC]
    public void DeactivateObjectRPC(int viewID)
    {
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
