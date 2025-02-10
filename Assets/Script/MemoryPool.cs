using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MemoryPool : MonoBehaviourPunCallbacks
{
    private class PoolItem
    {
        public GameObject gameObject;
        public PhotonView photonView;  // 네트워크 동기화를 위한 PhotonView 참조
    }

    private int increaseCount = 1; // 한번에 생성하는 오브젝트 개수
    private int maxCount;
    private int activeCount;

    private GameObject poolObjectPrefab; // 오브젝트 프리팹
    private List<PoolItem> poolItemList;

    public int MaxCount => maxCount;
    public int ActiveCount { 
        get => activeCount; 
        set => activeCount = value;
    }

    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObjectPrefab)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObjectPrefab = poolObjectPrefab;

        poolItemList = new List<PoolItem>();
    }

    /// <summary>
    /// 새로운 오브젝트 생성 (포톤 네트워크 동기화 포함)
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(poolObjectPrefab.name, tempPosition, Quaternion.identity);
            PhotonView photonView = obj.GetComponent<PhotonView>();

            PoolItem poolItem = new PoolItem
            {
                gameObject = obj,
                photonView = photonView
            };

            obj.SetActive(false);
            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// 특정 오브젝트를 풀에서 활성화
    /// </summary>
    public GameObject ActivatePoolItem(Vector3 position)
    {
        if (poolItemList == null) return null;

        if (maxCount == activeCount)
        {
            InstantiateObjects();
        }

        foreach (var poolItem in poolItemList)
        {
            if (!poolItem.gameObject.activeInHierarchy)
            {
                activeCount++;

                poolItem.gameObject.transform.position = position;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// 사용이 완료된 오브젝트를 비활성화 (RPC 호출)
    /// </summary>
    public void DeactivatePoolItem(GameObject obj)
    {
        if (obj == null) return;

        PhotonView pv = obj.GetComponent<PhotonView>();

        if (pv != null && pv.ViewID != 0)
        {
            pv.RPC("DeactivateObjectRPC", RpcTarget.AllBuffered, pv.ViewID);
        }
        else
        {
            Debug.LogWarning($"[MemoryPool] DeactivatePoolItem: PhotonView is null or ViewID is invalid. Deactivating locally.");
            obj.SetActive(false); // 로컬에서만 비활성화
        }
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

    /// <summary>
    /// 모든 오브젝트 제거 (게임 종료 시)
    /// </summary>
    public void DestroyObjects()
    {
        foreach (var poolItem in poolItemList)
        {
            if (poolItem.gameObject != null)
            {
                PhotonNetwork.Destroy(poolItem.gameObject);
            }
        }

        poolItemList.Clear();
    }
}
