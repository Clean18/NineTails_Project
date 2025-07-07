using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RangeMonsterFSM : MonoBehaviour, IDamagable
{
    enum MonsterState { Idle, Move, Attack }

    [Header("Monster Status")]
    [SerializeField] private float MoveSpeed = 2f;          // 이동 속도
    [SerializeField] private float DetectRange = 5f;        // 플레이어를 감지하는 범위
    [SerializeField] private float AttackRange = 5f;        // 공격 가능한 거리
    [SerializeField] private float AttackCooldown = 2f;     // 공격 쿨다운 시간
    [SerializeField] private float MaxHp = 10f;             // 몬스터의 최대 체력
    public float CurrentHp;               // 몬스터의 현재 체력
    [SerializeField] private float DamageReduceRate = 0f;   // 몬스터의 데미지 감소율

    [Header("Search Player Cooldown")]
    [SerializeField] private float FindInterval = 1.0f;         // 플레이어 재탐색 주기

    [Header("FSM Control")]
    [SerializeField] private float StateChangeCooldown = 1f;    // 상태 전환 쿨타임
    private float StateChangeTimer = 0f;


    [Header("Attack Setting")]
    [SerializeField] private GameObject AttackEffectPrefabs;    // 공격 이펙트 프리팹
    [SerializeField] private Transform AttackPoint;             // 공격 기준 위치
    [SerializeField] private GameObject ProjectilePrefabs;      // 투사체 프리팹
    [SerializeField] private float ProjectileSpeed = 8f;        // 투사체 속도
    [SerializeField] private int AttackDamage = 10;          // 공격 데미지
    [SerializeField] private AudioClip AttackSound;             // 공격 사운드
    [SerializeField] private Animator MonsterAnimator;          // 애니메이터

    private MonsterState _currentState = MonsterState.Idle; // 현재 FSM 상태
    private float _findTimer;         // 플레이어 재탐색 타이머
    private Transform targetPlayer;   // 현재 타겟 플레이어
    private Coroutine AttackRoutine;  // 공격 코루틴 저장 변수

    [SerializeField] private long warmthAmount;
    [SerializeField] private long spiritEnergyAmount;

    private void Start()
    {
        CurrentHp = MaxHp;
    }

    private void Update()
    {
        // 추가
        if (GameManager.Instance.Player == null) return;

        _findTimer += Time.deltaTime;
        StateChangeTimer += Time.deltaTime;     //  쿨타임의 타이머 증가

        // 일정 주기마다 가장 가까운 플레이어 탐색
        if (_findTimer >= FindInterval)
        {
            FindClosestPlayer();
            _findTimer = 0;
        }

        // 타겟 플레이어가 없으면 Idle 상태 유지
        if (targetPlayer == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        // 상태 변환 조건
        if (StateChangeTimer >= StateChangeCooldown)    // 상태 전환 쿨타임이 지난 경우에만 상태 변경 가능하게 조건 추가
        {
            switch (_currentState)
            {
                case MonsterState.Idle:

                    if (dist < DetectRange)
                    {
                        ChangeState(MonsterState.Move);
                    }

                    break;
                case MonsterState.Move:

                    if (dist < AttackRange)
                    {
                        ChangeState(MonsterState.Attack);
                    }
                    else if (dist >= DetectRange)
                    {
                        ChangeState(MonsterState.Idle);
                    }

                    break;

                case MonsterState.Attack:

                    if (dist > AttackRange)
                    {
                        ChangeState(MonsterState.Move);
                    }

                    break;

            }
        }

        // 각 몬스터의 상태 별 행동 처리

        switch (_currentState)
        {
            case MonsterState.Idle:
                // 대기상태 
                break;
            case MonsterState.Move:
                MoveToPlayer();
                FaceTarget();
                break;
            case MonsterState.Attack:
                FaceTarget();
                break;
        }

    }

    void OnEnable() => CurrentHp = MaxHp;
    void OnDisable() => targetPlayer = null;

    /// <summary>
    /// 현재 플레이어를 가장 가까운 대상으로 설정
    /// </summary>
    private void FindClosestPlayer()
    {
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Transform Closest = null;
        //float minDist = float.MaxValue;

        //foreach (var playerobj in players)
        //{
        //    if (!playerobj.activeInHierarchy) continue;

        //    float dist = Vector2.Distance(transform.position, playerobj.transform.position);
        //    if (dist < minDist)
        //    {
        //        minDist = dist;
        //        Closest = playerobj.transform;
        //    }
        //}

        //targetPlayer = Closest;
        targetPlayer = GameManager.Instance.Player.transform;
    }

    private void ChangeState(MonsterState newstate)
    {
        if (_currentState == newstate) return;

        //Debug.Log($"몬스터의 행동 상태 변경 : {_currentState} 에서 {newstate} 로 변경됨");

        // 이전 상태가 공격이면 코루틴 정지
        if (_currentState == MonsterState.Attack && AttackRoutine != null)
        {
            StopCoroutine(AttackRoutine);
            AttackRoutine = null;
        }

        _currentState = newstate;
        StateChangeTimer = 0f;      // 상태 변경 시 쿨타임 초기화

        // 새로운 상태가 공격이면 코루틴을 실행
        if (newstate == MonsterState.Attack && AttackRoutine == null)
        {
            AttackRoutine = StartCoroutine(AttackRoutineCoroutine());
        }
    }

    /// <summary>
    /// 이동 처리
    /// </summary> 
    private void MoveToPlayer()
    {
        Vector2 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += (Vector3)dir * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 몬스터가 바라보는 방향을 플레이어 쪽으로 조정
    /// </summary>
    private void FaceTarget()
    {
        if (targetPlayer == null) return;

        Vector2 dir = targetPlayer.position - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
            transform.localScale = scale;
        }
    }

    private IEnumerator AttackRoutineCoroutine()
    {
        while (_currentState == MonsterState.Attack)
        {
            // 애니메이션
            if (MonsterAnimator != null)
            {
                MonsterAnimator.SetTrigger("Attack");
            }

            // 사운드
            if (AttackSound != null)
            {
                AudioSource.PlayClipAtPoint(AttackSound, transform.position);
            }

            // 이펙트
            if (AttackEffectPrefabs != null && AttackPoint != null)
            {
                GameObject fx = Instantiate(AttackEffectPrefabs, AttackPoint.position, Quaternion.identity);
                Destroy(fx, 1f); // 1초 뒤 삭제
            }

            // 몬스터의 공격으로 인한 플레이어의 피격 판정
            // 투사체 발사
            if (ProjectilePrefabs != null && AttackPoint != null && targetPlayer != null)
            {
                Vector2 dir = (targetPlayer.position - AttackPoint.position).normalized;

                GameObject projectile = Instantiate(ProjectilePrefabs, AttackPoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {                    
                    rb.AddForce(dir * ProjectileSpeed, ForceMode2D.Impulse);
                }

                // MonsterProjectile 컴포넌트에 데미지 설정
                MonsterProjectile mp = projectile.GetComponent<MonsterProjectile>();
                if (mp != null)
                {
                    mp.SetDamage(AttackDamage);
                }
            }

            yield return new WaitForSeconds(AttackCooldown);
        }

        AttackRoutine = null;
    }
    /// <summary>
    /// 피격 처리
    /// </summary>
    public void TakeDamage(long damage)
    {
        long finalDamage = (long)(damage * (1f - DamageReduceRate / 100f));
        CurrentHp -= finalDamage;

        Debug.Log($" 플레이어가 몬스터에게 가한 데미지 {damage}, 데미지 감소율이 적용되어 몬스터가 입은 피해 : {finalDamage}  남은 체력 : {CurrentHp}");

        UIManager.Instance.ShowDamageText(transform, damage);

        if (CurrentHp <= 0) Die();
    }
    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("몬스터 사망함");
        // TODO : 플레이어 재화 증가
        GameManager.Instance.Player.AddCost(CostType.Warmth, warmthAmount); // 온기는 랜덤으로
        GameManager.Instance.Player.AddCost(CostType.SpiritEnergy, spiritEnergyAmount);
        MissionManager.Instance.AddKill(); // 돌파미션 킬 체크
        string stageId = SceneManager.GetActiveScene().name;    // 현재 씬이름을 스테이지Id로 사용
        AchievementManager.Instance?.KillCount(stageId);        // 해당 씬 킬 업적 체크
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 디버그용 범위 시각화
    /// </summary>
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, DetectRange);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, AttackRange);

    //    if (AttackPoint != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    //    }
    //}
}
