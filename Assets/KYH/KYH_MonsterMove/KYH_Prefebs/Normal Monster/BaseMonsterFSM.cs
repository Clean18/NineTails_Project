using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// 몬스터 공통 FSM (Finite State Machine) 부모 클래스
// 근접/원거리 몬스터 모두 이 클래스를 상속하여 상태 관리 로직을 공통 처리
public abstract class BaseMonsterFSM : MonoBehaviour, IDamagable
{
    // 몬스터의 상태를 정의하는 열거형
    protected enum MonsterState { Idle, Move, Attack, Dead }

    // 몬스터의 타입을 정의하는 열거형 

    [Header("Monster Status")]
    [SerializeField] protected float MoveSpeed = 2f;            // 이동 속도
    public float AttackRange = 1.5f;                            // 공격 사거리 (공통)
    [SerializeField] protected float AttackCooldown = 2f;       // 공격 쿨타임 (공통)
    [SerializeField] protected float MaxHp = 10f;               // 최대 체력
    [SerializeField] protected float DamageReduceRate = 0f;     // 데미지 감소율 (퍼센트)
    public float CurrentHp;                                     // 현재 체력
    [SerializeField] protected int AttackDamage;
    [SerializeField] protected MonsterType Type;
    [SerializeField] protected int Level;

    [Header("FSM Control")]
    [SerializeField] protected float FindInterval = 1f;         // 플레이어 탐색 주기 (초)
    [SerializeField] protected float StateChangeCooldown = 1f;  // 상태 전환 쿨타임
    protected float _findTimer;                                 // 탐색 타이머
    protected float _stateChangeTimer;                          // 상태 전환 쿨타이머

    [Header("Reward")]
    [SerializeField] protected long warmthAmount;               // 온기 보상량
    [SerializeField] protected long spiritEnergyAmount;         // 정기 보상량

    [SerializeField] protected MonsterState _currentState = MonsterState.Idle;   // 현재 FSM 상태
    protected Transform targetPlayer;                           // 타겟팅된 플레이어
    protected Coroutine attackRoutine;                          // 공격 루틴 저장

    [SerializeField] protected AudioMixerGroup sfxMixerGroup;
    protected AudioSource sfxAudioSource;

    [SerializeField] protected SpriteRenderer _sprite;

    // HpBar 추가
    [SerializeField] private MonsterHpBar hpBar;

    protected virtual void Awake()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.loop = false;

        _sprite = GetComponent<SpriteRenderer>();

        // HpBar 추가
        hpBar = GetComponentInChildren<MonsterHpBar>();
        if (hpBar != null)
            hpBar.SetHealth(CurrentHp, MaxHp);
    }

    protected void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxAudioSource == null) return;
        sfxAudioSource.PlayOneShot(clip, volume);
    }

    // 초기화
    protected virtual void OnEnable()
    {
        // 초기화
        CurrentHp = MaxHp; // 현재 체력을 최대체력으로 초기화
        _isFadingOut = false;
        _currentState = MonsterState.Idle;
        // 알파값 초기화
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 255);
        // 체력바 초기화
        hpBar.SetHealth(CurrentHp, MaxHp);
    }

    // 매 프레임마다 상태 업데이트
    protected virtual void Update()
    {
        // 플레이어가 없거나 이미 죽은 상태면 아무것도 하지 않음
        if (GameManager.Instance.Player == null || _currentState == MonsterState.Dead) return;

        _findTimer += Time.deltaTime;          // 탐색 타이머 증가
        _stateChangeTimer += Time.deltaTime;   // 상태 변경 타이머 증가

        // 일정 시간마다 플레이어 탐색
        if (_findTimer >= FindInterval)
        {
            FindClosestPlayer();  // 플레이어 위치 갱신
            _findTimer = 0f;
        }

        // 타겟이 없으면 Idle 상태 유지
        if (targetPlayer == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // 타겟과 거리 측정
        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        // 상태 전환 조건 확인
        if (_stateChangeTimer >= StateChangeCooldown)
        {
            switch (_currentState)
            {
                case MonsterState.Idle:
                    if (targetPlayer != null)
                        ChangeState(MonsterState.Move); // 타겟 있으면 무조건 이동
                    break;

                case MonsterState.Move:
                    if (dist < AttackRange)
                        ChangeState(MonsterState.Attack); // 사거리 안이면 공격
                    break;

                case MonsterState.Attack:
                    if (dist > AttackRange)
                        ChangeState(MonsterState.Move); // 사거리 벗어나면 다시 이동
                    break;
            }
        }

        // 상태에 따라 행동 수행
        switch (_currentState)
        {
            case MonsterState.Idle:
                // 아무것도 하지 않음
                break;

            case MonsterState.Move:
                MoveToPlayer();  // 플레이어에게 이동
                FaceTarget();    // 방향 전환
                break;

            case MonsterState.Attack:
                FaceTarget();    // 공격 중에도 방향 고정
                break;
        }
    }

    // 플레이어에게 이동
    protected virtual void MoveToPlayer()
    {
        if (targetPlayer == null) return;
        Vector2 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += (Vector3)dir * MoveSpeed * Time.deltaTime;
        
    }

    // 플레이어를 바라보게 방향 회전
    protected void FaceTarget()
    {
        if (targetPlayer == null) return;
        Vector3 dir = targetPlayer.position - transform.position;
        //Debug.Log("방향전환");
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x); // 왼/오 방향만 전환
            transform.localScale = scale;
        }
    }

    // 상태 전환 처리
    protected virtual void ChangeState(MonsterState newState)
    {
        if (_currentState == newState) return;

        // 이전 상태가 공격이면 공격 루틴 중단
        if (_currentState == MonsterState.Attack && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        _currentState = newState;
        _stateChangeTimer = 0f; // 상태 변경 쿨타임 리셋

        // 새 상태가 공격이면 공격 루틴 시작
        //if (newState == MonsterState.Attack && attackRoutine == null)
        //{
        //    attackRoutine = StartCoroutine(AttackRoutine());
        //}
    }

    // 가장 가까운 플레이어 탐색 (여기선 단일 플레이어 고정)
    protected virtual void FindClosestPlayer()
    {
        targetPlayer = GameManager.Instance.Player?.transform;
    }

    // 피격 처리 (데미지 감소 적용)
    public virtual void TakeDamage(long damage)
    {
        if (CurrentHp <= 0)
        {
            return;
        }
        long finalDamage = (long)(damage * (1f - DamageReduceRate / 100f));
        CurrentHp -= finalDamage;

        Debug.Log($"[공통] 받은 피해: {finalDamage}, 남은 체력: {CurrentHp}");
        UIManager.Instance.ShowDamageText(transform, damage); // 데미지 텍스트 출력

        //체력바 추가
        if (hpBar != null)
            hpBar.SetHealth(CurrentHp, MaxHp);

        if (CurrentHp <= 0) Die();
    }

    // 사망 처리
    protected virtual void Die()
    {
        if (_currentState == MonsterState.Dead) return; // 중복 방지

        Debug.Log("몬스터 사망");

        _currentState = MonsterState.Dead; // 상태를 Dead로 변경

        // 플레이어 보상 지급
        // 100%
        GameManager.Instance.Player.AddCost(CostType.Warmth, warmthAmount);
        // 10%
        if (Random.Range(0, 100) < 10) GameManager.Instance.Player.AddCost(CostType.SpiritEnergy, spiritEnergyAmount);

        // 미션 처리
        MissionManager.Instance.AddKill();

        // 오브젝트 비활성화
        // gameObject.SetActive(false);
    }

    // 몬스터가 천천히 투명해지며 사라지는 처리
    protected IEnumerator FadeOutAndDestroy()
    {
        // 이미 실행 중이라면 더 이상 실행되지 않도록 차단
        if (_isFadingOut) yield break;
        _isFadingOut = true;

        // 자식 오브젝트 중에서 SpriteRenderer를 가져온다
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        // SpriteRenderer가 없으면 그냥 즉시 삭제
        if (sr == null)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            yield break;
        }

        // 총 사라지는 데 걸릴 시간 (1.5초 동안 천천히 사라짐)
        float duration = 1.5f;

        // 경과 시간 변수
        float elapsed = 0f;

        // 원래 색상을 저장 (알파 값 포함)
        Color originalColor = sr.color;

        // 반복문을 통해 시간이 지나면서 알파 값을 점점 낮춘다
        while (elapsed < duration)
        {
            // 프레임 간 경과 시간을 누적
            elapsed += Time.deltaTime;

            // 현재 시간에 비례해서 알파 값을 줄이기 위한 비율 계산
            float ratio = elapsed / duration;

            // 알파 값을 직접 계산: 처음엔 1 → 점점 0으로
            float newAlpha = 1f - ratio;

            // 알파 값을 적용한 새 색상 만들기
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            // SpriteRenderer에 색상 적용
            sr.color = newColor;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 다 사라졌으면 최종적으로 오브젝트 제거
        //Destroy(gameObject);
        
        gameObject.SetActive(false);
    }

    // 코루틴 중복 실행 방지용 플래그 변수
    private bool _isFadingOut = false;

    // 공격 루틴은 자식 클래스에서 반드시 오버라이드 해야 함
    protected abstract IEnumerator AttackRoutine();

    public void MonsterDataInit(MonsterData data)
    {
        Type = data.Type;
        AttackDamage = data.Attack;
        Level = data.Level;
        MaxHp = data.MaxHp;
        CurrentHp = MaxHp;
        warmthAmount = data.DropWarmth;
        spiritEnergyAmount = data.DropSpiritEnergy;
    }

    public abstract void MonsterAttackStart();
}
