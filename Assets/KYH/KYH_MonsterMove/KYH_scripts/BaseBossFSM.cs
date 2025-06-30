using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 보스몬스터 FSM의 공통 부모 클래스.
/// 체력, 상태머신 흐름, 데미지 처리, 상태 전환 등의 공통 로직을 담당하며,
/// 패턴1,2,3의 세부 구현은 자식 클래스에서 오버라이드하여 정의한다.
/// </summary>
public abstract class BaseBossFSM : MonoBehaviour, IDamagable
{
    // 보스의 상태를 정의하는 열거형
    protected enum BossState
    {
        Null,
        Intro,      // 등장 연출
        Idle,       // 대기 상태 (패턴 전환 준비 상태)
        Pattern1,   // 일반 공격 패턴 1
        Pattern2,   // 일반 공격 패턴 2
        Pattern3,   // 특수 공격 패턴
        Dead        // 체력이 0 이하가되면 사망
    }

    [Header("Boss HP")]
    [SerializeField] protected float MaxHealth = 1000f;     // 최대 체력
    [SerializeField] protected float CurrentHealth;         // 현재 체력
    [SerializeField] protected float DamageReduceRate = 0f; // 몬스터의 데미지 감소율

    protected BossState CurrentState;         // 현재 FSM 상태
    protected bool isDeadHandled = false;     // 죽음 처리 중복 방지용

    [Header("FSM Timer")]
    [SerializeField] protected float IdleTime = 3f;         // Idle 상태에서 대기하는 시간 (다음 패턴 전환까지 딜레이)
    protected float IdleTimer;                             // Idle 상태에서 누적된 시간

    [SerializeField] protected Transform PlayerTransform;  // 플레이어 트랜스폼

    protected Coroutine BossPatternRoutine;                // 현재 실행 중인 패턴 코루틴 참조
    protected virtual int PatternCount => 3;
    protected Vector3 originalPosition;                               // 초기 위치 저장
    // 시작 시 상태 초기화
    protected virtual void Start()
    {
        CurrentState = BossState.Null;
        StartCoroutine(BossInit());
        originalPosition = transform.position;
    }

    // 게임 매니저에서 플레이어 트랜스폼이 준비되었을 때까지 대기 후 초기화
    protected IEnumerator BossInit()
    {
        yield return new WaitUntil(() => GameManager.Instance?.PlayerController != null);
        PlayerTransform = GameManager.Instance.PlayerController.transform;

        CurrentHealth = MaxHealth;              // 보스 체력 초기화
        TransitionToState(BossState.Intro);     // Intro 상태로 시작
    }

    // 매 프레임 상태에 따라서 처리
    protected virtual void Update()
    {
        if (CurrentState == BossState.Null) return;

        switch (CurrentState)
        {
            case BossState.Intro: HandleIntro(); break;
            case BossState.Idle: HandleIdle(); break;
            case BossState.Pattern1: HandlePattern1(); break;
            case BossState.Pattern2: HandlePattern2(); break;
            case BossState.Pattern3: HandlePattern3(); break;
            case BossState.Dead: HandleDead(); break;
        }
    }

    /// <summary>
    /// 상태 전환 처리 함수
    /// </summary>
    protected void TransitionToState(BossState newState)
    {
        CurrentState = newState;
        IdleTimer = 0f; // 상태 변화 시 Idle 타이머 초기화
        Debug.Log($"보스 몬스터 상태가 {newState} 상태로 전환됨");
    }

    /// <summary>
    /// Intro 상태 처리 (연출 후 Idle 전환)
    /// </summary>
    protected virtual void HandleIntro()
    {
        TransitionToState(BossState.Idle);
    }

    /// <summary>
    /// Idle 상태 처리 (일정 시간 후 랜덤 패턴으로 전환)
    /// </summary>
    protected virtual void HandleIdle()
    {
        IdleTimer += Time.deltaTime;

        if (IdleTimer >= IdleTime)
        {
            IdleTimer = 0f;
            int rand = Random.Range(0, PatternCount); // 0~2 중 선택

            switch (rand)
            {
                case 0: TransitionToState(BossState.Pattern1); break;
                case 1: TransitionToState(BossState.Pattern2); break;
                case 2: TransitionToState(BossState.Pattern3); break;
            }
        }
    }

    /// <summary>
    /// 패턴1 - 일반 공격 1 처리 (자식 클래스에서 구현)
    /// </summary>
    protected abstract void HandlePattern1();

    /// <summary>
    /// 패턴2 - 일반 공격 2 처리 (자식 클래스에서 구현)
    /// </summary>
    protected abstract void HandlePattern2();

    /// <summary>
    /// 패턴3 - 특수 공격 처리 (자식 클래스에서 구현)
    /// </summary>
    protected abstract void HandlePattern3();

    /// <summary>
    /// 사망 상태 처리
    /// </summary>
    protected virtual void HandleDead()
    {
        if (!isDeadHandled)
        {
            isDeadHandled = true;
            Debug.Log("보스 사망 연출 시작");
            Invoke(nameof(DestroySelf), 3f); // 3초 후 제거
        }
    }

    /// <summary>
    /// 보스 오브젝트 제거
    /// </summary>
    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// IDamagable 인터페이스 구현 - 데미지 처리
    /// </summary>
    public virtual void TakeDamage(long damage)
    {
        if (CurrentState == BossState.Dead) return;

        float finalDamage = damage * (1f - DamageReduceRate / 100f);
        CurrentHealth -= finalDamage;

        Debug.Log($"플레이어가 보스에게 데미지 {damage} 를 입힘, 실제 피해 : {finalDamage} | 남은 체력: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            TransitionToState(BossState.Dead);
        }
    }
}
