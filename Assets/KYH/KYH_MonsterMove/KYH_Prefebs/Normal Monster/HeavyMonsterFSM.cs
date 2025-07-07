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
        {
            Debug.Log("죽어서 진행안함");
            yield break;
        }

        // 공격 상태일 동안 반복 실행
        while (_currentState == MonsterState.Attack)
        {
            // 1. 애니메이션 재생 (Attack 트리거)
            //MonsterAnimator.Play("Attack_Tree");
            MonsterAnimator.Play("Tanker_Attack");

            // 2. 공격 사운드 재생
            PlaySound(AttackSound);

            //  // 3. 이펙트 생성 (공격 시점에)
            //  if (AttackEffectPrefab != null && AttackPoint != null)
            //  {
            //      GameObject fx = Instantiate(AttackEffectPrefab, AttackPoint.position, Quaternion.identity);
            //      Destroy(fx, 1f); // 1초 뒤 자동 삭제
            //  }

            // 4. 범위 내 플레이어 탐지 및 데미지 처리
            Collider2D hit = Physics2D.OverlapCircle(AttackPoint.position, AttackRadius, PlayerLayer);
            if (hit != null)
            {
                // 실제론 플레이어마다 다르게 처리해야 하지만 단일 플레이어 전제하에 아래로 단순화
                GameManager.Instance.Player.TakeDamage((long)AttackDamage);
            }

            // 5. 공격 쿨타임 대기 후 다시 루프
            yield return new WaitForSeconds(AttackCooldown);
        }

        // 상태 변경으로 루프 종료되면 코루틴 정리
        attackRoutine = null;

        // MonsterAnimator.Play("Idle_Tree");
        // MonsterAnimator.Play("Tanker_Idle");
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
            //Debug.Log("탱커 몬스터 공격 실행");
            //attackRoutine = StartCoroutine(AttackRoutine());
            // 애니메이션 이벤트 함수에서 공격 실행
            MonsterAnimator.Play("Tanker_Attack");
            // 애니메이션 이벤트 함수에는 AttackRoutine 등록
        }
    }
}

