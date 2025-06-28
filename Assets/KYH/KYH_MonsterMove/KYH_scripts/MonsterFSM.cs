using UnityEngine;
using System.Collections;
public class MonsterFSM : MonoBehaviour, IDamagable
{
    // 몬스터의 상태를 나타내는 열거형
    enum MonsterState { Idle, Move, Attack }

    [Header("Monster Status")]
    [SerializeField] private float MoveSpeed = 2f;          // 이동 속도
    [SerializeField] private float DetectRange = 5f;        // 플레이어를 감지하는 범위
    [SerializeField] private float AttackRange = 1.5f;      // 공격 가능한 거리
    [SerializeField] private float AttackCooldown = 2f;     // 공격 쿨다운 시간
    [SerializeField] private float MaxHp = 10f;             // 몬스터의 최대 체력
    [SerializeField] private float CurrentHp;               // 몬스터의 현재 체력
    [SerializeField] private float DamageReduceRate = 0f;   // 몬스터의 데미지 감소율

    [Header("Search Player Cooldown")]
    [SerializeField] private float FindInterval = 1.0f;     // 플레이어 재탐색 주기

    [Header("FSM Control")]
    [SerializeField] private float StateChangeColldown = 1f;    // 상태 전환의 쿨타임 변수
    private float StateChangeTimer = 0f;

    [Header("Attack Setting")]
    [SerializeField] private GameObject AttackEffectPrefab; // 공격 이펙트 프리팹
    [SerializeField] private Transform AttackPoint;         // 공격 기준 위치
    [SerializeField] private float AttackRadius = 1f;       // 공격 범위 반지름
    [SerializeField] private float AttackDamage = 10f;      // 공격 데미지
    [SerializeField] private AudioClip AttackSound;         // 공격 사운드
    [SerializeField] private Animator MonsterAnimator;      // 애니메이터

    private MonsterState _currentState = MonsterState.Idle; // 현재 FSM 상태
    private float _findTimer;         // 플레이어 재탐색 타이머
    private Transform targetPlayer;   // 현재 타겟 플레이어
    private Coroutine AttackRoutine;  // 공격 코루틴 저장 변수

    [SerializeField] private LayerMask _playerLayer; // 플레이어 레이어

    private void Start()
    {
        CurrentHp = MaxHp; // 체력 초기화
    }

    private void Update()
    {
        _findTimer += Time.deltaTime;
        StateChangeTimer += Time.deltaTime;     // 상태전환 쿨타임의 타이머 증가

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
        if (StateChangeTimer >= StateChangeColldown)
        {
            switch (_currentState)
            {
                case MonsterState.Idle:
                    if (dist < DetectRange)
                        ChangeState(MonsterState.Move);
                    break;

                case MonsterState.Move:
                    if (dist < AttackRange)
                        ChangeState(MonsterState.Attack);
                    else if (dist >= DetectRange)
                        ChangeState(MonsterState.Idle);
                    break;

                case MonsterState.Attack:
                    if (dist > AttackRange)
                        ChangeState(MonsterState.Move);
                    break;
            }
        }

        // 상태별 행동 처리
        switch (_currentState)
        {
            case MonsterState.Idle:
                // 대기 상태
                break;

            case MonsterState.Move:
                MoveToPlayer();
                FaceTarget();
                break;

            case MonsterState.Attack:
                FaceTarget(); // 방향 유지
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
        //Transform closest = null;
        //float minDist = float.MaxValue;

        //foreach (var playerobj in players)
        //{
        //    if (!playerobj.activeInHierarchy) continue;

        //    float dist = Vector2.Distance(transform.position, playerobj.transform.position);
        //    if (dist < minDist)
        //    {
        //        minDist = dist;
        //        closest = playerobj.transform;
        //    }
        //}

        //targetPlayer = closest;
        targetPlayer = GameManager.Instance.PlayerController.transform;
    }

    /// <summary>
    /// 상태 변경 처리 + 공격 코루틴 관리
    /// </summary>
    private void ChangeState(MonsterState newstate)
    {
        if (_currentState == newstate) return;

        //Debug.Log($"몬스터의 행동 상태 변경 : {_currentState} 에서 {newstate}로 변경됨");

        // 이전 상태가 공격이면 코루틴 정지
        if (_currentState == MonsterState.Attack && AttackRoutine != null)
        {
            StopCoroutine(AttackRoutine);
            AttackRoutine = null;
        }

        _currentState = newstate;
        StateChangeTimer = 0f;

        // 새로운 상태가 공격이면 코루틴 시작
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
    /// 방향을 플레이어 쪽으로 조정
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

    /// <summary>
    /// 공격 코루틴 (이펙트, 애니메이션, 판정 포함)
    /// </summary>
    private IEnumerator AttackRoutineCoroutine()
    {
        while (_currentState == MonsterState.Attack)
        {
            // 애니메이션
            if (MonsterAnimator != null)
                MonsterAnimator.SetTrigger("Attack");

            // 사운드
            if (AttackSound != null)
                AudioSource.PlayClipAtPoint(AttackSound, transform.position);

            // 이펙트
            if (AttackEffectPrefab != null && AttackPoint != null)
            {
                GameObject fx = Instantiate(AttackEffectPrefab, AttackPoint.position, Quaternion.identity);
                Destroy(fx, 1f); // 1초 후 삭제
            }

            // 피격 판정
            Collider2D[] hits = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRadius, _playerLayer);
            foreach (var hit in hits)
            {
                //Game.Data.PlayerData player = hit.GetComponent<Game.Data.PlayerData>();
                var player = GameManager.Instance.PlayerController;
                if (player != null)
                {
                    player.TakeDamage((long)AttackDamage);
                    //Debug.Log($" 몬스터가 플레이어에게 {(long)AttackDamage}피해를 입힘!");
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

        Debug.Log($"플레이어가 몬스터에게 가한 피해 {damage}, 몬스터가 실제로 받은 피해 {finalDamage},  현재 남은 체력 : {CurrentHp}");

        UIManager.Instance.ShowDamageText(transform, damage);

        if (CurrentHp <= 0)
        {
            //Die();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(gameObject);
    }

    /// <summary>
    /// 디버그용 범위 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        if (AttackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
        }
    }
}
