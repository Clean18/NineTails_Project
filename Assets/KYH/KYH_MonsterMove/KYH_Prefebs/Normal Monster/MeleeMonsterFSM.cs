using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterFSM : BaseMonsterFSM
{
    [Header("Attack Setting")]
    [SerializeField] private GameObject AttackEffectPrefab;
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private float AttackRadius = 1f;
    [SerializeField] private float AttackDamage = 10f;
    [SerializeField] private LayerMask PlayerLayer;
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

            Collider2D[] hits = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRadius, PlayerLayer);
            foreach (var hit in hits)
            {
                GameManager.Instance.PlayerController.TakeDamage((long)AttackDamage);
            }

            yield return new WaitForSeconds(AttackCooldown);
        }

        attackRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
        }
    }
}
