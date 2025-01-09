using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Photon.Pun;

public class MemoryPool : MonoBehaviourPunCallbacks
{
    // �޸� Ǯ�� �����Ǵ� ������Ʈ ����
    private class PoolItem
    {
        public bool isActive;               // "gameObject"�� Ȱ��ȭ/��Ȱ��ȭ ����
        public GameObject gameObject;       // ȭ�鿡 ���̴� ���� ���� ������Ʈ
    }

    private int increaseCount = 1;          // ������Ʈ�� ������ �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
    private int maxCount;                   // ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ����
    private int activeCount;                // ���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ) ������Ʈ ����

    private GameObject poolObject;          // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������

    private List<PoolItem> poolItemList;    // �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ

    public int MaxCount => maxCount;        // �ܺο��� ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int ActiveCount => activeCount;  // �ܺο��� ���� Ȱ��ȭ �Ǿ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    // ������Ʈ�� �ӽ÷� �����Ǵ� ��ġ
    private Vector3 tempPosition = new Vector3(48, 1, 48);

    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        //InstantiateObjects();
    }

    /// <summary>
    /// increaseCount ������ ������Ʈ�� ����
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; ++i)
        {
            GameObject obj = PhotonNetwork.Instantiate(poolObject.name, tempPosition, Quaternion.identity);

            PoolItem poolItem = new PoolItem
            {
                isActive = false,
                gameObject = obj
            };

            obj.SetActive(false); // ��Ȱ��ȭ
            poolItemList.Add(poolItem);
        }
        
    }

    /// <summary>
    /// ���� ��������(Ȱ��/��Ȱ��) ��� ������Ʈ�� ����
    /// </summary>
    public void DestoryObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }


    /// <summary>
    /// poolItemList�� ����Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ�ؼ� ���
    /// ���� ��� ������Ʈ�� ������̸� InstatiateObjeects()�� �߰� ����
    /// </summary>
    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null) return null;

        // ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
        // ��� ������Ʈ�� Ȱ��ȭ �����̸� ���ο� ������Ʈ �ʿ�
        if( maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.isActive == false)
            {
                activeCount++;

                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    [PunRPC]
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    /// <summary>
    /// ���ӿ� ������� ��� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    public void DeactivateAllPoolItem(GameObject removeObject)
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void DeactivateObjectRPC(GameObject obj)
    {
        //PhotonNetwork.Destroy(gameObject);
        obj.SetActive(false);
    }
}
