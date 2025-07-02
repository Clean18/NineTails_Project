using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMonsterFSM : BaseMonsterFSM
{
    [Header("Attack Setting")]
    [SerializeField] private GameObject AttackEffectPrefab;
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private float ProjectileSpeed = 8f;
    [SerializeField] private float AttackDamage = 10f;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private Animator MonsterAnimator;

    protected override IEnumerator AttackRoutine()
    {
        while (_currentState == MonsterState.Attack)
        {
            MonsterAnimator?.SetTrigger("Attack");
            AudioSource.PlayClipAtPoint(AttackSound, transform.position);

            if (AttackEffectPrefab != null && AttackPoint != null)
            {
                GameObject fx = Instantiate(AttackEffectPrefab, AttackPoint.position, Quaternion.identity);
                Destroy(fx, 1f);
            }

            if (ProjectilePrefab != null && AttackPoint != null && targetPlayer != null)
            {
                Vector2 dir = (targetPlayer.position - AttackPoint.position).normalized;
                GameObject projectile = Instantiate(ProjectilePrefab, AttackPoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb?.AddForce(dir * ProjectileSpeed, ForceMode2D.Impulse);

                MonsterProjectile mp = projectile.GetComponent<MonsterProjectile>();
                if (mp != null)
                {
                    mp.SetDamage(AttackDamage);
                }
            }

            yield return new WaitForSeconds(AttackCooldown);
        }

        attackRoutine = null;
    }
}
