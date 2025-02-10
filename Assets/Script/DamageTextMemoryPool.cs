using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextMemoryPool : MonoBehaviour
{
    public static DamageTextMemoryPool instance;

    [SerializeField]
    private GameObject textPrefab;      // 텍스트 프리팹
    private MemoryPool textPool;        // 텍스트 메모리 풀

    [SerializeField] private Transform texts; // 관리할 부모 오브젝트

    private void Awake()
    {
        instance = this;
        textPool = new MemoryPool(textPrefab);
    }

    public void SpawnText(float Damage, bool critical, Vector3 position)
    {
        GameObject Text = textPool.ActivatePoolItem(position);
        Text.transform.SetParent(texts);
        Text.transform.position = position;
        Text.GetComponentInChildren<DamageText>().SetUp(textPool, Damage, critical);
    }
}
