using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonsterFSM : MonoBehaviour, IDamagable
{
    protected enum MonsterState { Idle, Move, Attack }

    [Header("Monster Status")]
    [SerializeField] protected float MoveSpeed = 2f;
    [SerializeField] protected float DetectRange = 5f;
    [SerializeField] protected float AttackRange = 1.5f;
    [SerializeField] protected float AttackCooldown = 2f;
    [SerializeField] protected float MaxHp = 10f;
    [SerializeField] protected float DamageReduceRate = 0f;
    public float CurrentHp;

    [Header("FSM Control")]
    [SerializeField] protected float FindInterval = 1f;
    [SerializeField] protected float StateChangeCooldown = 1f;
    protected float _findTimer;
    protected float _stateChangeTimer;

    [Header("Reward")]
    [SerializeField] protected long warmthAmount;
    [SerializeField] protected long spiritEnergyAmount;

    protected MonsterState _currentState = MonsterState.Idle;
    protected Transform targetPlayer;
    protected Coroutine attackRoutine;

    protected virtual void Start()
    {
        CurrentHp = MaxHp;
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.PlayerController == null) return;

        _findTimer += Time.deltaTime;
        _stateChangeTimer += Time.deltaTime;

        if (_findTimer >= FindInterval)
        {
            FindClosestPlayer();
            _findTimer = 0f;
        }

        if (targetPlayer == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        if (_stateChangeTimer >= StateChangeCooldown)
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

        switch (_currentState)
        {
            case MonsterState.Idle: break;
            case MonsterState.Move:
                MoveToPlayer();
                FaceTarget();
                break;
            case MonsterState.Attack:
                FaceTarget();
                break;
        }
    }

    protected void MoveToPlayer()
    {
        if (targetPlayer == null) return;
        Vector2 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += (Vector3)dir * MoveSpeed * Time.deltaTime;
    }

    protected void FaceTarget()
    {
        if (targetPlayer == null) return;
        Vector3 dir = targetPlayer.position - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
            transform.localScale = scale;
        }
    }

    protected virtual void ChangeState(MonsterState newState)
    {
        if (_currentState == newState) return;

        if (_currentState == MonsterState.Attack && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        _currentState = newState;
        _stateChangeTimer = 0f;

        if (newState == MonsterState.Attack && attackRoutine == null)
        {
            attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    protected virtual void FindClosestPlayer()
    {
        targetPlayer = GameManager.Instance.PlayerController?.transform;
    }

    public virtual void TakeDamage(long damage)
    {
        long finalDamage = (long)(damage * (1f - DamageReduceRate / 100f));
        CurrentHp -= finalDamage;

        Debug.Log($"[공통] 받은 피해: {finalDamage}, 남은 체력: {CurrentHp}");
        UIManager.Instance.ShowDamageText(transform, damage);

        if (CurrentHp <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log("몬스터 사망");
        GameManager.Instance.PlayerController.AddCost(CostType.Warmth, warmthAmount);
        GameManager.Instance.PlayerController.AddCost(CostType.SpiritEnergy, spiritEnergyAmount);
        MissionManager.Instance.AddKill();
        gameObject.SetActive(false);
    }

    // 공격 루틴은 자식이 오버라이드
    protected abstract IEnumerator AttackRoutine();
}
