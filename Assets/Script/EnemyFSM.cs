using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, Death, Skill }
public enum EnemyType { minion, archor , warrior}

/// <summary>
/// Enemy ��ȸ ��ũ��Ʈ
/// </summary>
public class EnemyFSM : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType = EnemyType.minion;

    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;           // �ν� ���� (�� ���� �ȿ� ������ "Pursuit" ���·� ����)
    [SerializeField]
    private float pursuitLimitRange = 10;               // ���� ���� (�� ���� �ٱ����� ������ "Wander" ���·� ����)
    [SerializeField]
    private float skillRange = 6;                       // ��ų ����

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;                // �߻�ü ������
    [SerializeField]
    private Transform projectileSpawnPoint;             // �߻�ü ���� ��ġ
    [SerializeField]
    private AttackCollider attackCollider;              // ���� ���� ����
    [SerializeField]
    private float attackRange = 5;                      // ���� ���� (�� ���� �ȿ� ������ "Attack" ���·� ����) 
    [SerializeField]
    private float attackRate = 1;                       // ���� �ӵ�
    [SerializeField]
    private float skillRate = 10;                       // ��ų �ֱ�

    [SerializeField]
    private EnemyState enemyState = EnemyState.None;    // ���� �� �ൿ

    [SerializeField]  private float lastAttackTime = 0; // ���� �ֱ� ���� ����
    [SerializeField]  private float lastSkillTime = 0;  // Ư�� ���� �ֱ� ���� ����

    [SerializeField]
    private HealthBar enemyHpBar;
    private Status status;                              // �̵��ӵ� ���� ����

    [SerializeField] private Collider[] colliders;       // ��Ʈ �ݶ��̴�

    [SerializeField]
    private NavMeshAgent navMeshAgent;                  // �̵� ��� ���� NavMeshAgent
    [SerializeField]
    private Animator animator;                          // �ִϸ��̼� ��� ���� Animator
    private Transform target;                           // ���� ���� ��� (�÷��̾�)
    private EnemyMemoryPool enemyMemoryPool;            // �� �޸� Ǯ (�� ������Ʈ ��Ȱ��ȭ�� ���)

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        //navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        // NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
        navMeshAgent.updateRotation = false;

        CollidersAble(true);

        enemyHpBar.Setup(status.MaxHP);
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ�� �� ���� ���¸� "���"�� ����
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // ���� ��Ȱ��ȭ�� �� ���� ������� ���¸� �����ϰ�, ���¸� "None"���� ����
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // ���� ������� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 ���� ������ return
        if (enemyState == newState) return;

        // ������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        // ���� ���� ���¸�newState�� ����
        enemyState = newState;
        // ���ο� ����  ���
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // �ִϸ��̼��� ���� ����
        animator.SetFloat("Movement", 0);

        // n�� �Ŀ� "��ȸ" ���·� �����ϴ� �ڷ�ƾ ����
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            // "���" ������ �� �ϴ� �ൿ
            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�� �ð� ���
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�κ���
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // �ִϸ��̼��� �ȱ�� ����
        animator.SetFloat("Movement", 0.5f);

        // �̵� �ӵ� ����
        navMeshAgent.speed = status.WalkSpeed;

        // ��ǥ ��ġ ����
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // ��ǥ ��ġ�� ȸ��
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            // ��ǥ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if((to-from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // ���¸� "���"�� ����
                ChangeState(EnemyState.Idle);
            }

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;    // ���� ��ġ�� �������� �ϴ� ���� ������
        int wanderJitter = 0;     // ���õ� ���� (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;    // �ּ� ����
        int wanderJitterMax = 360;  // �ִ� ����

        // ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ�� (������ ��� �ൿ�� ���� �ʵ���)
        Vector3 rangetPosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // �ڽ��� ��ġ�� �߽����� ������(wanderRadius) �Ÿ�, ���õ� ����(wanderJitter)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangetPosition.x - rangeScale.x * 0.5f, rangetPosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangetPosition.z - rangeScale.z * 0.5f, rangetPosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        targetRecognitionRange = 64;

        while (true)
        {
            // �̵� �ӵ� ���� (��ȸ�� ���� �ȴ� �ӵ��� �̵�. ������ ���� �ٴ� �ӵ��� �̵�)
            navMeshAgent.speed = status.RunSpeed;

            // �ִϸ��̼��� �ٱ�� ����
            animator.SetFloat("Movement", 1f);

            // ��ǥ��ġ�� ���� �÷��̾��� ��ġ�� ����
            navMeshAgent.SetDestination(target.position);

            // Ÿ�� ������ ��� �ֽ��ϵ��� ��
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Death()
    {
        // ������ �̵��� ����
        navMeshAgent.speed = 0;
        animator.SetTrigger("Death");
        yield return null;
    }

    private IEnumerator Attack()
    {
        if (enemyType != EnemyType.warrior)
        // �ִϸ��̼��� ���� ����
        animator.SetFloat("Movement", 0);

        // ������ ���� �̵��� ���ߵ��� ����
        navMeshAgent.speed = 0;

        while (true)
        {
            // Ÿ�� ���� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();
            if (Time.time - lastAttackTime > attackRate)
            {
                // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
                lastAttackTime = Time.time;

                // ���� �ִϸ��̼� ���
                animator.SetFloat("AttackStrategy", 0f);
                animator.Play("Attack", -1, 0);
            }

            yield return null;
        }
    }

    private IEnumerator Skill()
    {
        if (enemyType != EnemyType.warrior)
            // �ִϸ��̼��� ���� ����
            animator.SetFloat("Movement", 0);

        // ������ ���� �̵��� ���ߵ��� ����
        navMeshAgent.speed = 0;
        while (true)
        {
            // Ÿ�� ���� �ֽ�
            LookRotationToTarget();

            // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
            CalculateDistanceToTargetAndSelectState();

            // ���� �ִϸ��̼� ���
            animator.SetFloat("AttackStrategy", 1f);
            animator.Play("Attack", -1, 0);
            StartCoroutine(JumpToTarget(1f, 2f));

            yield return null;
        }
    }

    // ���� �ڷ�ƾ
    private IEnumerator JumpToTarget(float duration, float height)
    {
        Vector3 startPosition = transform.position; // ���� ��ġ
        Vector3 endPosition = target.position;
        float elapsedTime = 0f; // ��� �ð�

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 0���� 1�� ���ϴ� ����

            // ������ ��� ���
            float heightOffset = height * Mathf.Sin(Mathf.PI * t); // ������ ����
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t); // ���� ����
            currentPosition.y += heightOffset; // y���� ���� �߰�

            transform.position = currentPosition; // ������Ʈ ��ġ ����

            yield return null; // ���� �����ӱ��� ���
        }

        transform.position = endPosition; // ���� ��ġ ����
        
        // Ÿ�ٰ��� �Ÿ��� ���� �ൿ ���� (��ȸ, �߰�, ���Ÿ� ����)
        CalculateDistanceToTargetAndSelectState();
    }

    private void LookRotationToTarget()
    {
        // ��ǥ ��ġ
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // �� ��ġ
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        if(enemyType != EnemyType.warrior)
        {
            // �ٷ� ����
            transform.rotation = Quaternion.LookRotation(to - from);
        }
        else
        {
            // ������ ����
            Quaternion rotation = Quaternion.LookRotation(to - from);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;

        // �÷��̾�(Target)�� ���� �Ÿ� ��� �� �Ÿ��� ���� �ൿ ����
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= skillRange && (Time.time - lastSkillTime > skillRate))
        {   
            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastSkillTime = Time.time;

            ChangeState(EnemyState.Skill);
        }
        else if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if( distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if  (distance >= targetRecognitionRange)
        {
            ChangeState(EnemyState.Wander);
        }

    }

    public void AttackFunction()
    {
        switch (enemyType)
        {
            case EnemyType.minion:
            case EnemyType.warrior:
                attackCollider.StartCollider(status.Damage);
                break;
            case EnemyType.archor:
                // �߻�ü ����
                Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                break;
        }

        
    }

    private void OnDrawGizmos()
    {
        // "��ȸ" ������ �� �̵��� ��� ǥ��
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        // ��ǥ �ν� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // ���� ����
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        // ���� ����
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ��ų ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }

    public bool TakeDamage(float damage)
    {
        targetRecognitionRange = 64;
        bool isDie = status.DecreaseHP(damage);
        enemyHpBar.takeDamage(damage);
        if (isDie)
        {
            CollidersAble(false);
            ChangeState(EnemyState.Death);
            return false;
        }
        return true;
    }

    public void Destory()
    {
        enemyMemoryPool.DeactivateEnemy(gameObject);
    }

    private void CollidersAble(bool b)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = b;
        }
    }
}
