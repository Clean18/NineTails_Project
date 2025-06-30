using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Stage2 보스 FSM - 패턴은 2개만 존재 (Pattern1, Pattern2)
/// BaseBossFSM을 상속받아 필요한 패턴만 구현
/// </summary>
public class Stage2BossFSM : BaseBossFSM
{
    [Header("Pattern1 설정 - 세로 발구르기 (2회, 60% 피해)")]
    [SerializeField] private Animator BossAnimator;
    [SerializeField] private GameObject WarningLineVertical;
    [SerializeField] private Transform WarningOrigin1;
    [SerializeField] private GameObject DustEffect1;
    [SerializeField] private AudioClip RoarSound1;
    [SerializeField] private float Pattern1Delay = 3f;
    [SerializeField] private Vector2 Pattern1BoxSize = new Vector2(2f, 6f);

    [Header("Pattern2 설정 - 방망이 수평 강타 (1회, 60% 피해)")]
    [SerializeField] private GameObject WarningLineHorizontal;
    [SerializeField] private Transform WarningOrigin2;
    [SerializeField] private GameObject DustEffect2;
    [SerializeField] private AudioClip RoarSound2;
    [SerializeField] private float Pattern2Delay = 3f;
    [SerializeField] private Vector2 Pattern2BoxSize = new Vector2(6f, 2f);

    protected override void HandlePattern1()
    {
        if (BossPatternRoutine == null)
            BossPatternRoutine = StartCoroutine(Pattern1Coroutine());
    }

    private IEnumerator Pattern1Coroutine()
    {
        Debug.Log("Stage2 패턴1 - 세로 발구르기 2회 시작");

        // 1. 울부짖는 연출
        BossAnimator.Play("Boss_Roar");
        AudioSource.PlayClipAtPoint(RoarSound1, transform.position);

        // 2. 경고 범위 표시 (세로 방향)
        GameObject warning = Instantiate(
            WarningLineVertical,
            WarningOrigin1.position,
            WarningOrigin1.rotation
        );

        // 3. 경고 유지 시간 대기
        yield return new WaitForSeconds(Pattern1Delay);

        // 4. 1차 타격
        BossAnimator.Play("Boss_Stomp1");
        if (DustEffect1 != null)
            Instantiate(DustEffect1, WarningOrigin1.position, Quaternion.identity);

        DealBoxDamage(WarningOrigin1.position, Pattern1BoxSize, 0.3f);

        yield return new WaitForSeconds(1f);

        // 5. 2차 타격
        BossAnimator.Play("Boss_Stomp2");
        if (DustEffect1 != null)
            Instantiate(DustEffect1, WarningOrigin1.position, Quaternion.identity);

        DealBoxDamage(WarningOrigin1.position, Pattern1BoxSize, 0.3f);

        // 6. 후처리
        yield return new WaitForSeconds(1f);
        if (warning != null) Destroy(warning);

        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    protected override void HandlePattern2()
    {
        if (BossPatternRoutine == null)
            BossPatternRoutine = StartCoroutine(Pattern2Coroutine());
    }

    private IEnumerator Pattern2Coroutine()
    {
        Debug.Log("Stage2 패턴2 - 방망이 수평 강타 시작");

        // 1. 발 두 번 구르고 울부짖는 연출
        BossAnimator.Play("Boss_StompStart");
        AudioSource.PlayClipAtPoint(RoarSound2, transform.position);

        // 2. 경고 범위 표시 (가로 방향)
        GameObject warning = Instantiate(
            WarningLineHorizontal,
            WarningOrigin2.position,
            WarningOrigin2.rotation
        );

        // 3. 경고 유지 시간 대기
        yield return new WaitForSeconds(Pattern2Delay);

        // 4. 방망이 강타
        BossAnimator.Play("Boss_HammerSmash");
        if (DustEffect2 != null)
            Instantiate(DustEffect2, WarningOrigin2.position, Quaternion.identity);

        DealBoxDamage(WarningOrigin2.position, Pattern2BoxSize, 0.6f);

        // 5. 후처리
        yield return new WaitForSeconds(1f);
        if (warning != null) Destroy(warning);

        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    protected override void HandlePattern3()
    {
        // Stage2 보스는 패턴3 없음
    }

    /// <summary>
    /// 범위 내 플레이어에게 박스 범위 데미지
    /// </summary>
    private void DealBoxDamage(Vector2 center, Vector2 size, float damagePercent)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            var player = hit.GetComponent<Game.Data.PlayerData>();
            if (player != null)
            {
                player.TakeDamageByPercent(damagePercent);
                Debug.Log($"Stage2 - 플레이어 {hit.name}에게 {damagePercent * 100f}% 데미지");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        // 패턴 1 - 세로 범위 박스
        if (WarningOrigin1 != null)
        {
            Gizmos.DrawCube(WarningOrigin1.position, Pattern1BoxSize);
            Handles.Label(WarningOrigin1.position + Vector3.up * 0.5f, "Pattern 1 범위");
        }

        // 패턴 2 - 가로 범위 박스
        if (WarningOrigin2 != null)
        {
            Gizmos.DrawCube(WarningOrigin2.position, Pattern2BoxSize);
            Handles.Label(WarningOrigin2.position + Vector3.up * 0.5f, "Pattern 2 범위");
        }
    }

    protected override int PatternCount => 2;
}

