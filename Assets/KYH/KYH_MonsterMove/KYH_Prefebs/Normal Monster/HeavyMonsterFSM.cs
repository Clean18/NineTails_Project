using System.Collections;
using UnityEngine;

public class HeavyMonsterFSM : BaseMonsterFSM
{
    [Header("Attack Setting")]
    // [SerializeField] private GameObject AttackEffectPrefab;  // 공격 시 생성할 이펙트 프리팹
    [SerializeField] private Transform AttackPoint;          // 공격 중심 위치 (예: 손, 발)
    [SerializeField] private float AttackRadius = 1f;        // 원형 공격 범위 반지름
    [SerializeField] private LayerMask PlayerLayer;          // 플레이어 판정용 레이어
    [SerializeField] private AudioClip AttackSound;          // 공격 시 재생할 사운드
    [SerializeField] private AudioClip HitSound;
    [SerializeField] private AudioClip DeadSound;              // 죽을때 사운드
    [SerializeField] private Animator MonsterAnimator;       // 애니메이터 컴포넌트


    protected override void MoveToPlayer()
    {
        base.MoveToPlayer();
        //MonsterAnimator.Play("Move_Tree");
        MonsterAnimator.Play("Tanker_Walk");
    }
    // 부모 클래스에서 정의된 추상 메서드 → 근접 공격 루틴 구현
    protected override IEnumerator AttackRoutine()
    {
        if (_currentState == MonsterState.Dead)
            yield break;

        while (_currentState == MonsterState.Attack)
        {
            MonsterAnimator.Play("Tanker_Attack");
            PlaySound(AttackSound);

            // 이 타이밍에서는 애니메이션만 재생하고
            // 데미지는 이벤트에서만
            yield return new WaitForSeconds(AttackCooldown);
        }

        attackRoutine = null;
        MonsterAnimator.Play("Tanker_Walk");
    }

    public void TankerAttackDamage()
    {
        if (_currentState != MonsterState.Attack) return;

        Collider2D hit = Physics2D.OverlapCircle(AttackPoint.position, AttackRadius, PlayerLayer);
        if (hit != null)
        {
            GameManager.Instance.Player.TakeDamage((long)AttackDamage);
            PlaySound(AttackSound);
        }
    }

    public override void TakeDamage(long damage)
    {
        base.TakeDamage(damage); // 부모의 공통 데미지 처리 로직 호출

        // 피격 사운드 재생
        PlaySound(HitSound); // BaseMonsterFSM에 정의된 PlaySound(clip)
    }

    protected override void Die()
    {
        base.Die();
        //MonsterAnimator.Play("Dead_Tree");
        MonsterAnimator.Play("Tanker_Die");
        PlaySound(DeadSound);

        StartCoroutine(FadeOutAndDestroy()); // 천천히 사라짐
        // 오브젝트 비활성화
        // Destroy(gameObject, 0.5f);
    }

    // Unity 편집기에서 공격 범위 확인용 Gizmo 그리기
    //private void OnDrawGizmosSelected()
    //{
    //    if (AttackPoint != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
    //    }
    //}
    protected override void ChangeState(MonsterState newState)
    {
        base.ChangeState(newState);
        // 상태가 변경됐을 때 공격 실행
        MonsterAttackStart();
    }
    public override void MonsterAttackStart()
    {
        if (_currentState == MonsterState.Attack/* && attackRoutine == null*/)
        {
            Debug.Log("탱커 몬스터 공격 실행");
            //attackRoutine = StartCoroutine(AttackRoutine());
            // 애니메이션 이벤트 함수에서 공격 실행
            MonsterAnimator.Play("Tanker_Attack");
            // 애니메이션 이벤트 함수에는 AttackRoutine 등록
        }
    }
}

