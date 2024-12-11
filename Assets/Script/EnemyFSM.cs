using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, Death, Skill }
public enum EnemyType { minion, archor , warrior}

/// <summary>
/// Enemy 배회 스크립트
/// </summary>
public class EnemyFSM : MonoBehaviour
{
    [SerializeField]
    private EnemyType enemyType = EnemyType.minion;

    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;           // 인식 범위 (이 범위 안에 들어오면 "Pursuit" 상태로 변경)
    [SerializeField]
    private float pursuitLimitRange = 10;               // 추적 범위 (이 범위 바깥으로 나가면 "Wander" 상태로 변경)
    [SerializeField]
    private float skillRange = 6;                       // 스킬 범위

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;                // 발사체 프리팹
    [SerializeField]
    private Transform projectileSpawnPoint;             // 발사체 생성 위치
    [SerializeField]
    private AttackCollider attackCollider;              // 근접 공격 범위
    [SerializeField]
    private float attackRange = 5;                      // 공격 범위 (이 범위 안에 들어오면 "Attack" 상태로 변경) 
    [SerializeField]
    private float attackRate = 1;                       // 공격 속도
    [SerializeField]
    private float skillRate = 10;                       // 스킬 주기

    [SerializeField]
    private EnemyState enemyState = EnemyState.None;    // 현재 적 행동

    [SerializeField]  private float lastAttackTime = 0; // 공격 주기 계산용 변수
    [SerializeField]  private float lastSkillTime = 0;  // 특수 공격 주기 계산용 변수

    [SerializeField]
    private HealthBar enemyHpBar;
    private Status status;                              // 이동속도 등의 정보

    [SerializeField] private Collider[] colliders;       // 히트 콜라이더

    [SerializeField]
    private NavMeshAgent navMeshAgent;                  // 이동 제어를 위한 NavMeshAgent
    [SerializeField]
    private Animator animator;                          // 애니메이션 제어를 위한 Animator
    private Transform target;                           // 적의 공격 대상 (플레이어)
    private EnemyMemoryPool enemyMemoryPool;            // 적 메모리 풀 (적 오브젝트 비활성화에 사용)

    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
    {
        status = GetComponent<Status>();
        //navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        // NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
        navMeshAgent.updateRotation = false;

        CollidersAble(true);

        enemyHpBar.Setup(status.MaxHP);
    }

    private void OnEnable()
    {
        // 적이 활성화될 때 적의 상태를 "대기"로 설정
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // 적이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
        StopCoroutine(enemyState.ToString());

        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (enemyState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        // 현재 적의 상태를newState로 설정
        enemyState = newState;
        // 새로운 상태  재생
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // 애니메이션을 대기로 설정
        animator.SetFloat("Movement", 0);

        // n초 후에 "배회" 상태로 변경하는 코루틴 실행
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            // "대기" 상태일 때 하는 행동
            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로변경
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // 애니메이션을 걷기로 설정
        animator.SetFloat("Movement", 0.5f);

        // 이동 속도 설정
        navMeshAgent.speed = status.WalkSpeed;

        // 목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            // 목표위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있으면
            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if((to-from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // 상태를 "대기"로 변경
                ChangeState(EnemyState.Idle);
            }

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;    // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;     // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;    // 최소 각도
        int wanderJitterMax = 360;  // 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기 (구역을 벗어난 행동을 하지 않도록)
        Vector3 rangetPosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
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
            // 이동 속도 설정 (배회할 때는 걷는 속도로 이동. 추적할 때는 뛰는 속도로 이동)
            navMeshAgent.speed = status.RunSpeed;

            // 애니메이션을 뛰기로 설정
            animator.SetFloat("Movement", 1f);

            // 목표위치를 현재 플레이어의 위치로 설정
            navMeshAgent.SetDestination(target.position);

            // 타겟 방향을 계속 주시하도록 함
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Death()
    {
        // 죽을때 이동을 멈춤
        navMeshAgent.speed = 0;
        animator.SetTrigger("Death");
        yield return null;
    }

    private IEnumerator Attack()
    {
        if (enemyType != EnemyType.warrior)
        // 애니메이션을 대기로 설정
        animator.SetFloat("Movement", 0);

        // 공격할 때는 이동을 멈추도록 설정
        navMeshAgent.speed = 0;

        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();
            if (Time.time - lastAttackTime > attackRate)
            {
                // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
                lastAttackTime = Time.time;

                // 공격 애니메이션 재생
                animator.SetFloat("AttackStrategy", 0f);
                animator.Play("Attack", -1, 0);
            }

            yield return null;
        }
    }

    private IEnumerator Skill()
    {
        if (enemyType != EnemyType.warrior)
            // 애니메이션을 대기로 설정
            animator.SetFloat("Movement", 0);

        // 공격할 때는 이동을 멈추도록 설정
        navMeshAgent.speed = 0;
        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectState();

            // 공격 애니메이션 재생
            animator.SetFloat("AttackStrategy", 1f);
            animator.Play("Attack", -1, 0);
            StartCoroutine(JumpToTarget(1f, 2f));

            yield return null;
        }
    }

    // 점프 코루틴
    private IEnumerator JumpToTarget(float duration, float height)
    {
        Vector3 startPosition = transform.position; // 시작 위치
        Vector3 endPosition = target.position;
        float elapsedTime = 0f; // 경과 시간

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 0에서 1로 변하는 비율

            // 포물선 경로 계산
            float heightOffset = height * Mathf.Sin(Mathf.PI * t); // 포물선 높이
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t); // 선형 보간
            currentPosition.y += heightOffset; // y값에 높이 추가

            transform.position = currentPosition; // 오브젝트 위치 설정

            yield return null; // 다음 프레임까지 대기
        }

        transform.position = endPosition; // 최종 위치 고정
        
        // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
        CalculateDistanceToTargetAndSelectState();
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.position.x, 0, target.position.z);
        // 내 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        if(enemyType != EnemyType.warrior)
        {
            // 바로 돌기
            transform.rotation = Quaternion.LookRotation(to - from);
        }
        else
        {
            // 서서히 돌기
            Quaternion rotation = Quaternion.LookRotation(to - from);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }
    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;

        // 플레이어(Target)와 적의 거리 계산 후 거리에 따라 행동 선택
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= skillRange && (Time.time - lastSkillTime > skillRate))
        {   
            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
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
                // 발사체 생성
                Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                break;
        }

        
    }

    private void OnDrawGizmos()
    {
        // "배회" 상태일 때 이동할 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        // 목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 추적 범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        // 공격 범위
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 스킬 범위
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
