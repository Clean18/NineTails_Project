using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 공격 몬스터 FSM 클래스
// BaseMonsterFSM을 상속하여 공격 루틴만 투사체 발사 방식으로 오버라이드
public class RangedMonsterFSM : BaseMonsterFSM
{
    [Header("Attack Setting")]
   // [SerializeField] private GameObject AttackEffectPrefab;    // 공격 시 생성되는 이펙트 프리팹
    [SerializeField] private Transform AttackPoint;            // 투사체가 생성될 위치
    [SerializeField] private GameObject ProjectilePrefab;      // 발사할 투사체 프리팹
    [SerializeField] private float ProjectileSpeed = 8f;       // 투사체 속도
    [SerializeField] private AudioClip AttackSound;            // 공격 사운드
    [SerializeField] private AudioClip HitSound;               // 피격 사운드
    [SerializeField] private AudioClip DeadSound;              // 죽을때 사운드
    [SerializeField] private Animator MonsterAnimator;         // 애니메이터


    protected override void MoveToPlayer()
    {
        base.MoveToPlayer();
        //MonsterAnimator.Play("Walk_Ranged");
        MonsterAnimator.Play("Ranged_Walk");
    }
    // BaseMonsterFSM의 추상 공격 루틴 구현 (원거리 투사체 방식)
    protected override IEnumerator AttackRoutine()
    {
        // 공격 상태일 때 반복
        while (_currentState == MonsterState.Attack)
        {
            // 1. 애니메이션 실행
            //MonsterAnimator.Play("ThrowDagger");
            MonsterAnimator.Play("Ranged_Attack");

            // 2. 사운드 재생
            PlaySound(AttackSound);

            // // 3. 이펙트 생성 (화염, 연기 등)
            // if (AttackEffectPrefab != null && AttackPoint != null)
            // {
            //     GameObject fx = Instantiate(AttackEffectPrefab, AttackPoint.position, Quaternion.identity);
            //     Destroy(fx, 1f); // 이펙트 1초 뒤 자동 제거
            // }
            Debug.Log("투사체 제작 해야함");
            // 4. 투사체 생성 및 방향/속도 적용
            if (ProjectilePrefab != null && AttackPoint != null && targetPlayer != null)
            {
                Debug.Log("투사체 생성됨");
                // 4-1. 방향 계산 (플레이어 방향)
                Vector2 dir = (targetPlayer.position - AttackPoint.position).normalized;

                // 4-2. 투사체 생성
                GameObject projectile = Instantiate(ProjectilePrefab, AttackPoint.position, Quaternion.identity);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle + 180f);  // Z축 회전

                // 3. 좌우 방향에 따라 Y축 반전
                if (dir.x < 0f)  
                {
                    projectile.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else  
                {
                    projectile.transform.localScale = new Vector3(1f, -1f, 1f);
                }
                // 4-3. 물리 적용 (속도 방향 설정)
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb?.AddForce(dir * ProjectileSpeed, ForceMode2D.Impulse);

                // 4-4. 데미지 설정 (MonsterProjectile 스크립트가 있어야 함)
                MonsterProjectile mp = projectile.GetComponent<MonsterProjectile>();
                if (mp != null)
                {
                    mp.SetDamage(AttackDamage);
                }
            }

            // 5. 공격 쿨타임 후 반복
            yield return new WaitForSeconds(AttackCooldown);
        }

        // 상태가 Attack에서 벗어나면 코루틴 정리
        attackRoutine = null;

        //MonsterAnimator.Play("Idle_Ranged");
        MonsterAnimator.Play("Ranged_Idle");
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
        //MonsterAnimator.Play("Dead_Ranged");
        MonsterAnimator.Play("Ranged_Die");
        PlaySound(DeadSound);

        StartCoroutine(FadeOutAndDestroy()); // 천천히 사라짐
        // 오브젝트 비활성화
        // Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(AttackPoint.position, 0.2f); // 이펙트/발사 위치

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange); // 원거리 사거리
        }
    }
}
