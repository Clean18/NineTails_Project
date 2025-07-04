using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 근접형 몬스터 FSM - BaseMonsterFSM을 상속받아 근접 공격 로직만 구현
public class MeleeMonsterFSM : BaseMonsterFSM
{
    [Header("Attack Setting")]
   // [SerializeField] private GameObject AttackEffectPrefab;  // 공격 시 생성할 이펙트 프리팹
    [SerializeField] private Transform AttackPoint;          // 공격 중심 위치 (예: 손, 발)
    [SerializeField] private float AttackRadius = 1f;        // 원형 공격 범위 반지름
    [SerializeField] private float AttackDamage = 10f;       // 공격 시 입히는 데미지
    [SerializeField] private LayerMask PlayerLayer;          // 플레이어 판정용 레이어
    [SerializeField] private AudioClip AttackSound;          // 공격 시 재생할 사운드
    [SerializeField] private Animator MonsterAnimator;       // 애니메이터 컴포넌트


    protected override void MoveToPlayer()
    {
        base.MoveToPlayer();
        MonsterAnimator.Play("Melee_Move");
    }
    // 부모 클래스에서 정의된 추상 메서드 → 근접 공격 루틴 구현
    protected override IEnumerator AttackRoutine()
    {
        // 공격 상태일 동안 반복 실행
        while (_currentState == MonsterState.Attack)
        {
            // 1. 애니메이션 재생 (Attack 트리거)
            MonsterAnimator.Play("Melee_Attack");

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
                GameManager.Instance.PlayerController.TakeDamage((long)AttackDamage);
            }

            // 5. 공격 쿨타임 대기 후 다시 루프
            yield return new WaitForSeconds(AttackCooldown);
        }

        // 상태 변경으로 루프 종료되면 코루틴 정리
        attackRoutine = null;

        MonsterAnimator.Play("Melee_Move");
    }

    protected override void Die()
    {
        base.Die();

        // 오브젝트 비활성화
        Destroy(gameObject, 1f);
    }

    // Unity 편집기에서 공격 범위 확인용 Gizmo 그리기
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
        }
    }
}
