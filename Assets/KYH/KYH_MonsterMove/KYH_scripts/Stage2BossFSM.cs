using System.Collections;
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
    [SerializeField] private Transform DustEffect1SpawnRight;
    [SerializeField] private Transform DustEffect1SpawnLeft;
    [SerializeField] private AudioClip LegMoveSound;

    [Header("다리 트랜스폼")]
    [SerializeField] private Transform RightLeg;
    [SerializeField] private Transform LeftLeg;

    [Header("Pattern2 설정 - 방망이 수평 강타 (1회, 60% 피해)")]
    [SerializeField] private GameObject WarningLineHorizontal;
    [SerializeField] private Transform WarningOrigin2;
    [SerializeField] private GameObject DustEffect2;
    [SerializeField] private GameObject DustEffect3;
    [SerializeField] private AudioClip RoarSound2;
    [SerializeField] private AudioClip SwingBonkSound;
    [SerializeField] private float Pattern2Delay = 3f;
    [SerializeField] private Vector2 Pattern2BoxSize = new Vector2(6f, 2f);
    [SerializeField] private Transform SwingBonkPoint1;
    [SerializeField] private Transform SwingBonkPoint2;

    [Header("방망이 기준 트랜스폼")]
    [SerializeField] private Transform LongMenArm;

    protected override void HandlePattern1()
    {
        if (BossPatternRoutine == null)
            BossPatternRoutine = StartCoroutine(Pattern1Coroutine());
    }

    private IEnumerator Pattern1Coroutine()
    {
        Debug.Log("Stage2 패턴1 - 세로 발구르기 2회 시작");

        // 1. 울부짖기
        BossAnimator.Play("Boss_Roar");
        PlaySound(RoarSound1);

        // 2. 경고 범위 표시
        GameObject warning = Instantiate(
            WarningLineVertical,
            WarningOrigin1.position,
            WarningOrigin1.rotation
        );

        // 3. 대기 후 애니메이션 순차 재생 (실제 처리 → 애니메이션 이벤트로)
        yield return new WaitForSeconds(Pattern1Delay);

        BossAnimator.Play("Giant_StompRight");
        yield return new WaitForSeconds(1.5f);

        BossAnimator.Play("Giant_StompLeft");
        yield return new WaitForSeconds(1.5f);

        // 4. 경고 제거 및 상태 복귀
        if (warning != null) Destroy(warning);
        BossAnimator.Play("Giant_Idle");
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

        // 1. 연출
        BossAnimator.Play("Giant_LegMove");
        PlaySound(LegMoveSound);
        yield return new WaitForSeconds(0.5f);
        PlaySound(LegMoveSound);
        yield return new WaitForSeconds(0.5f);
        PlaySound(RoarSound2);

        // 2. 경고 표시
        GameObject warning = Instantiate(
            WarningLineHorizontal,
            WarningOrigin2.position,
            WarningOrigin2.rotation
        );

        yield return new WaitForSeconds(Pattern2Delay);

        // 3. 애니메이션 → 이펙트/데미지는 애니메이션 이벤트에서 실행
        BossAnimator.Play("Giant_Swing_Bonk");

        yield return new WaitForSeconds(2f);
        if (warning != null) Destroy(warning);

        BossAnimator.Play("Giant_Idle");
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    protected override void HandlePattern3()
    {
        // Stage2 보스는 패턴3 없음
    }

    // pattern1 - 오른발 구르기
    public void OnRightFootStomp()
    {
        if (DustEffect1 != null && DustEffect1SpawnRight != null)
        {
            GameObject dust = Instantiate(DustEffect1, DustEffect1SpawnRight.position, Quaternion.identity, transform);

            if (RightLeg != null)
                dust.transform.localScale = RightLeg.localScale;

            PlaySound(LegMoveSound);
            Destroy(dust, 2f);
        }

        DealBoxDamage(WarningOrigin1.position, Pattern1BoxSize, 0.3f);
    }

    // Pattern1 - 왼발 발구르기
    public void OnLeftFootStomp()
    {
        if (DustEffect1 != null && DustEffect1SpawnLeft != null)
        {
            GameObject dust = Instantiate(DustEffect1, DustEffect1SpawnLeft.position, Quaternion.identity, transform);

            if (LeftLeg != null)
                dust.transform.localScale = LeftLeg.localScale;

            PlaySound(LegMoveSound);
            Destroy(dust, 2f);
        }

        DealBoxDamage(WarningOrigin1.position, Pattern1BoxSize, 0.3f);
    }

    // Pattern2 - 방망이 강타
    public void OnHammerBonk()
    {
        if (DustEffect2 != null && DustEffect3 != null)
        {
            GameObject dust1 = Instantiate(DustEffect2, SwingBonkPoint1.position, Quaternion.identity);
            GameObject dust2 = Instantiate(DustEffect3, SwingBonkPoint2.position, Quaternion.identity);
            PlaySound(SwingBonkSound);
            Destroy(dust1, 2f);
            Destroy(dust2, 2f);
        }
        DealBoxDamage(WarningOrigin2.position, Pattern2BoxSize, 0.6f);
    }

    /// <summary>
    /// 범위 내 플레이어에게 박스 범위 데미지
    /// </summary>
    private void DealBoxDamage(Vector2 center, Vector2 size, float damagePercent)
    {
        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, _playerLayer);

        if (hit != null)
        {
            PlayerController.Instance.TakeDamage((long)(PlayerController.Instance.GetMaxHp() * damagePercent));
            Debug.Log($"Stage2 - 플레이어 {hit.name}에게 {damagePercent * 100f}% 데미지");
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (WarningOrigin1 != null)
    //    {
    //        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
    //        Gizmos.DrawCube(WarningOrigin1.position, Pattern1BoxSize);
    //        Handles.Label(WarningOrigin1.position + Vector3.up * 0.3f, "Pattern 1 데미지 범위");
    //    }

    //    if (WarningOrigin2 != null)
    //    {
    //        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
    //        Gizmos.DrawCube(WarningOrigin2.position, Pattern2BoxSize);
    //        Handles.Label(WarningOrigin2.position + Vector3.up * 0.3f, "Pattern 2 데미지 범위");
    //    }

    //    // Pattern1 이펙트 위치 시각화 (오른발)
    //    if (DustEffect1SpawnRight != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(DustEffect1SpawnRight.position, 0.2f);
    //        Handles.Label(DustEffect1SpawnRight.position + Vector3.up * 0.1f, "오른발 이펙트 위치");
    //    }

    //    // Pattern1 이펙트 위치 시각화 (왼발)
    //    if (DustEffect1SpawnLeft != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawSphere(DustEffect1SpawnLeft.position, 0.2f);
    //        Handles.Label(DustEffect1SpawnLeft.position + Vector3.up * 0.1f, "왼발 이펙트 위치");
    //    }
    //}

    protected override IEnumerator DeadRoutine()
    {
        yield return null;

        float timer = 0f;
        float duration = 3f;
        Color startColor = _sprite.color;

        while (timer < duration)
        {
            float t = timer / duration;

            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            _sprite.color = newColor;

            timer += Time.deltaTime;
            yield return null;
        }

        //Destroy(gameObject);
        gameObject.SetActive(false);

        Debug.Log("보스 2 사망 다음 씬으로 이동");
        SceneChangeManager.Instance.LoadNextScene();
    }

    protected override int PatternCount => 2;
}

