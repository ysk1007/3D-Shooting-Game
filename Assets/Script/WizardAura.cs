using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAura : MonoBehaviour
{
    [SerializeField] private float power = 10f;           // ȸ����
    [SerializeField] private float detectionRadius = 10f; // Ž�� �ݰ�
    [SerializeField] private float detectionInterval = 3f; // Ž�� ����

    private void OnEnable()
    {
        // ���� �������� Ž�� �ڷ�ƾ ����
        StartCoroutine(DetectEnemiesRoutine());
    }

    IEnumerator DetectEnemiesRoutine()
    {
        while (true)
        {
            DetectEnemies();
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    void DetectEnemies()
    {
        // Ž���� ������Ʈ ����
        Collider[] detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider obj in detectedObjects)
        {
            // Enemy �±װ� �ִ� ������Ʈ���� Ȯ��
            if (obj.CompareTag("EnemyFSM"))
            {
                // Status ��ũ��Ʈ�� ������ �ִ��� Ȯ��
                EnemyFSM enemy = obj.transform.GetComponent<EnemyFSM>();

                if (enemy == null) continue;
                if (enemy.EnemyState != EnemyState.Death) enemy.Healing(power);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // �����Ϳ��� Ž�� �ݰ��� �ð�ȭ
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
