using System.Diagnostics;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    // 몬스터의 상태를 나타내는 열거형
    enum MonsterState { Idle, Move, Attack }

    [Header("Monster status")]
    public float MoveSpeed = 2f;          // 이동 속도
    public float DetectRange = 5f;        // 플레이어를 감지하는 범위
    public float AttackRange = 1.5f;      // 공격 가능한 거리
    public float AttackCooldown = 2f;     // 공격 쿨다운 시간

    [Header("Search Player Cooldown")]
    public float FindInterval = 1.0f;     // 몇 초마다 플레이어를 재탐색할지 설정

    private MonsterState _currentState = MonsterState.Idle; // 현재 상태
    private float _attackTimer;       // 공격 쿨다운 타이머
    private float _findTimer;         // 플레이어 재탐색 타이머

    private Transform targetPlayer;   // 현재 추적 중인 플레이어

    private void Update()
    {
        // 플레이어 재탐색 타이머 증가
        _findTimer += Time.deltaTime;

        // 설정된 주기마다 플레이어 다시 탐색
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

        // 현재 플레이어와의 거리 계산
        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        // 상태 전이 조건 처리
        switch (_currentState)
        {
            case MonsterState.Idle:
                if (dist < DetectRange)
                    ChangeState(MonsterState.Move);
                break;

            case MonsterState.Move:
                if (dist < AttackRange)
                    ChangeState(MonsterState.Attack);
                else if (dist >= AttackRange)
                    ChangeState(MonsterState.Idle);
                break;

            case MonsterState.Attack:
                if (dist > AttackRange)
                    ChangeState(MonsterState.Move);
                break;

        }

        // 현재 상태에 따른 행동 실행
        switch (_currentState)
        {
            case MonsterState.Idle:
                // 아무 행동 안 함
                break;

            case MonsterState.Move:
                MoveToPlayer(); // 플레이어 쪽으로 이동
                break;

            case MonsterState.Attack:
                HandelAttack(); // 공격 처리
                break;
        }
    }

    /// <summary>
    /// 씬 전체에서 "Player" 태그를 가진 오브젝트들을 찾아
    /// 가장 가까운 플레이어를 추적 대상으로 설정
    /// </summary>
    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var playerobj in players)
        {
            if (!playerobj.activeInHierarchy) continue;

            float dist = Vector2.Distance(transform.position, playerobj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = playerobj.transform;
            }
        }

        targetPlayer = closest;
    }

    /// <summary>
    /// 상태 변경 처리 및 초기화
    /// </summary>
    void ChangeState(MonsterState newstate)
    {
        if (_currentState == newstate) return;
        UnityEngine.Debug.Log($"State changed : {_currentState} → {newstate}");
        _currentState = newstate;
        _attackTimer = 0f; // 공격 쿨다운 초기화
    }

    /// <summary>
    /// 플레이어 쪽으로 이동하는 로직
    /// </summary>
    void MoveToPlayer()
    {
        Vector2 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += (Vector3)dir * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 공격 로직: 쿨다운을 고려해 공격 실행
    /// </summary>
    void HandelAttack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= AttackCooldown)
        {
            UnityEngine.Debug.Log("몬스터가 플레이어를 공격!");
            // 여기서 실제로 데미지를 주는 함수 호출 가능
            _attackTimer = 0f;
        }
    }

    /// <summary>
    /// Unity Editor에서 감지/공격 범위 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
