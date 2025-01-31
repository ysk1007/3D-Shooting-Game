using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAura : MonoBehaviour
{
    [SerializeField] private float power = 10f;           // 회복량
    [SerializeField] private float detectionRadius = 10f; // 탐지 반경
    [SerializeField] private float detectionInterval = 3f; // 탐지 간격

    private void OnEnable()
    {
        // 일정 간격으로 탐지 코루틴 실행
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
        // 탐지된 오브젝트 저장
        Collider[] detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider obj in detectedObjects)
        {
            // Enemy 태그가 있는 오브젝트인지 확인
            if (obj.CompareTag("EnemyFSM"))
            {
                // Status 스크립트를 가지고 있는지 확인
                EnemyFSM enemy = obj.transform.GetComponent<EnemyFSM>();

                if (enemy == null) continue;
                if (enemy.EnemyState != EnemyState.Death) enemy.Healing(power);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // 에디터에서 탐지 반경을 시각화
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
