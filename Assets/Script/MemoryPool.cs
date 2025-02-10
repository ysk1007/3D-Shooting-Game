using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MemoryPool : MonoBehaviourPunCallbacks
{
    private class PoolItem
    {
        public GameObject gameObject;
        public PhotonView photonView;  // ��Ʈ��ũ ����ȭ�� ���� PhotonView ����
    }

    private int increaseCount = 1; // �ѹ��� �����ϴ� ������Ʈ ����
    private int maxCount;
    private int activeCount;

    private GameObject poolObjectPrefab; // ������Ʈ ������
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
    /// ���ο� ������Ʈ ���� (���� ��Ʈ��ũ ����ȭ ����)
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
    /// Ư�� ������Ʈ�� Ǯ���� Ȱ��ȭ
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
    /// ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ (RPC ȣ��)
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
            obj.SetActive(false); // ���ÿ����� ��Ȱ��ȭ
        }
    }

    /// <summary>
    /// ��� Ŭ���̾�Ʈ���� ������Ʈ ��Ȱ��ȭ
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
    /// ��� ������Ʈ ���� (���� ���� ��)
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
