using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextMemoryPool : MonoBehaviour
{
    public static DamageTextMemoryPool instance;

    [SerializeField]
    private GameObject textPrefab;      // �ؽ�Ʈ ������
    private MemoryPool textPool;        // �ؽ�Ʈ �޸� Ǯ

    [SerializeField] private Transform texts; // ������ �θ� ������Ʈ

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
