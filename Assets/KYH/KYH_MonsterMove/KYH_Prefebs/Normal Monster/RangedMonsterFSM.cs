using System.Collections;
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
        if (_currentState == MonsterState.Dead)
        {
            yield break;
        }
        while (_currentState == MonsterState.Attack)
        {
            MonsterAnimator.Play("Ranged_Attack");

            yield return new WaitForSeconds(AttackCooldown);
        }

        attackRoutine = null;
        MonsterAnimator.Play("Ranged_Idle");
    }

    // 애니메이션 이벤트로 호출됨
    public void FireProjectile()
    {
        if (ProjectilePrefab != null && AttackPoint != null && targetPlayer != null)
        {
            PlaySound(AttackSound);

            Vector2 dir = (targetPlayer.position - AttackPoint.position).normalized;

            GameObject projectile = Instantiate(ProjectilePrefab, AttackPoint.position, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);

            // 좌우에 따라 Y축 뒤집기
            if (dir.x < 0f)
                projectile.transform.localScale = new Vector3(1f, 1f, 1f);
            else
                projectile.transform.localScale = new Vector3(1f, -1f, 1f);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb?.AddForce(dir * ProjectileSpeed, ForceMode2D.Impulse);

            MonsterProjectile mp = projectile.GetComponent<MonsterProjectile>();
            if (mp != null)
            {
                mp.SetDamage(AttackDamage);
            }
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
        //MonsterAnimator.Play("Dead_Ranged");
        MonsterAnimator.Play("Ranged_Die");
        PlaySound(DeadSound);

        StartCoroutine(FadeOutAndDestroy()); // 천천히 사라짐
        // 오브젝트 비활성화
        // Destroy(gameObject, 0.5f);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (AttackPoint != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(AttackPoint.position, 0.2f); // 이펙트/발사 위치

    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position, AttackRange); // 원거리 사거리
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
            //Debug.Log("원거리 몬스터 공격 실행");
            //attackRoutine = StartCoroutine(AttackRoutine());
            // 애니메이션 이벤트 함수에서 공격 실행
            MonsterAnimator.Play("Ranged_Attack");
            // 애니메이션 이벤트 함수에는 AttackRoutine 등록
        }
    }
}
