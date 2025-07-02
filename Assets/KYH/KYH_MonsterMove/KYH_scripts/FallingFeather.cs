using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFeather : MonoBehaviour
{
    [SerializeField] Animator featherAnimator;

    [Header("데미지 설정")]
    [SerializeField] private float DamagePercent = 0.05f;
    [SerializeField] private float DamageRadius = 1.5f;
    [SerializeField] private string TargetTag = "Player";

    [Header("떨어지는 조건")]
    [SerializeField] private float StopThreshold = 0.2f;
    [SerializeField] private float DropForce = 15f;

    [Header("참조")]
    public Transform WarningPoint;

    private Rigidbody2D rb;
    private bool hasDealtDamage = false;
    private bool hasLaunched = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (WarningPoint != null)
        {
            Debug.Log("헤즈 런치드 트루 진행됨");
            Vector2 dir = (WarningPoint.position - transform.position).normalized;
            rb.AddForce(dir * DropForce, ForceMode2D.Impulse);
            hasLaunched = true;
        }

        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        Debug.Log("픽스 업데이트 들어옴");

        if (!hasLaunched || hasDealtDamage || WarningPoint == null) return;

        float featherY = transform.position.y;
        float warningY = WarningPoint.position.y;
        float delta = featherY - warningY;
        Debug.Log("픽스 업데이트 진행중");
        if (Mathf.Abs(delta) <= StopThreshold)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            DealDamage();
            hasDealtDamage = true;

            featherAnimator.Play("Fire_Feather");

            if (WarningPoint != null)
            {
                Debug.Log("경고위치 표기 지우기 시도함");
                Destroy(WarningPoint.gameObject, 1.5f);
            }

                Destroy(gameObject, 1.5f);
        }
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, DamageRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(TargetTag))
            {
                var player = hit.GetComponent<Game.Data.PlayerData>();
                if (player != null)
                {
                    player.TakeDamageByPercent(DamagePercent);
                    Debug.Log($"[FallingFeather] 플레이어 {hit.name}에게 {DamagePercent * 100}% 데미지");
                }
            }
        }
    }
}
