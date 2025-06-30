using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stage3 보스 FSM - 회오리, 깃털 낙하, 돌진 공격 패턴을 포함한 FSM 구현
/// BaseBossFSM을 상속하며 패턴마다 애니메이션, 사운드, 이펙트, 경고 UI, 데미지를 제어함
/// </summary>
public class Stage3BossFSM : BaseBossFSM
{
    [Header("Pattern1 - 회오리 공격")]
    [SerializeField] private GameObject WarningCirclePrefab;        // 회오리 패턴 경고 원 프리팹
    [SerializeField] private GameObject WhirlwindEffect;            // 회오리 이펙트 프리팹
    [SerializeField] private AudioClip RoarSound1;                  // 회오리 공격 전 포효 사운드
    [SerializeField] private float Pattern1Duration = 3f;           // 회오리 공격 지속 시간
    [SerializeField] private float Pattern1HitInterval = 0.5f;      // 회오리 데미지 판정 간격
    [SerializeField] private float Pattern1DamagePercent = 5f;      // 회오리 1회 데미지 비율(%)
    [SerializeField] private float Pattern1Range = 3f;              // 회오리 공격 범위 (원형 범위)
    [SerializeField] private Transform Pattern1Origin;              // 회오리 중심 위치 (보스 중심 또는 지정 지점)

    [Header("Pattern2 - 깃털 낙하")]
    [SerializeField] private GameObject WarningFeatherSpotPrefab;               // 깃털 경고 프리팹
    [SerializeField] private GameObject FeatherEffectPrefab;                    // 깃털 이펙트 프리팹
    [SerializeField] private AudioClip RoarSound2;                              // 깃털 공격 전 사운드
    [SerializeField] private float Pattern2Delay = 2f;                          // 경고 후 깃털 낙하까지 대기 시간
    [SerializeField] private float Pattern2DamagePercent = 5f;                  // 낙하한 깃털 데미지 비율(%)
    [SerializeField] private int FeatherCount = 6;                              // 깃털 낙하 수
    [SerializeField] private Vector2 Pattern2BoxSize = new Vector2(6f, 4f);     // (너비, 높이)
    [SerializeField] private Transform Pattern2Center;                          // 낙하 박스 중심 위치

    [Header("Pattern3 - 돌진 공격")]
    [SerializeField] private GameObject WarningRectPrefab;          // 돌진 경고 직사각형 프리팹
    [SerializeField] private GameObject AuraEffect;                 // 돌진 시 오라 이펙트
    [SerializeField] private AudioClip ChargeSound;                 // 돌진 사운드
    [SerializeField] private float ChargeDelay = 3f;                // 돌진 전 대기 시간
    [SerializeField] private float Pattern3DamagePercent = 70f;     // 돌진 충돌 시 데미지 비율(%)
    [SerializeField] private float ChargeForce = 20f;               // 보스 돌진 속도 (AddForce 계수)
    [SerializeField] private Transform ChargeStartPos;              // 돌진 시작 위치
    [SerializeField] private Transform ChargeEndPos;                // 돌진 도착 위치

    [SerializeField] private Animator BossAnimator;                 // 보스 애니메이터
    [SerializeField] private Rigidbody2D BossRigidbody;             // 보스 리지드바디 (돌진용)
    [SerializeField] private Collider2D ChargeCollider;             // 돌진용 충돌 콜라이더

    private bool isCharging = false;                                // 돌진 중인지 여부
    private bool hasChargedHit = false;                             // 돌진 충돌이 이미 발생했는지 여부

    protected override void HandlePattern1()
    {
        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern1Coroutine());
        }
    }

    protected override void HandlePattern2()
    {
        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern2Coroutine());
        }
    }

    protected override void HandlePattern3()
    {
        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern3Coroutine());
        }
    }

    /// <summary>
    /// 회오리 공격 패턴 - 일정 범위 안에서 여러 번 데미지를 입히는 회오리 공격
    /// </summary>
    private IEnumerator Pattern1Coroutine()
    {
        // 1. 경고 원 생성
        GameObject warning = Instantiate(WarningCirclePrefab, Pattern1Origin.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);

        // 2. 경고 제거 및 애니메이션+사운드 재생
        Destroy(warning);
        BossAnimator.Play("Boss_WingOpen");
        AudioSource.PlayClipAtPoint(RoarSound1, transform.position);

        // 3. 회오리 이펙트 생성
        GameObject whirlwind = Instantiate(WhirlwindEffect, Pattern1Origin.position, Quaternion.identity);

        // 4. 지속 시간 동안 일정 간격으로 데미지
        int hitCount = Mathf.FloorToInt(Pattern1Duration / Pattern1HitInterval);
        for (int i = 0; i < hitCount; i++)
        {
            DealDamageInCircle(Pattern1Origin.position, Pattern1Range, Pattern1DamagePercent);
            yield return new WaitForSeconds(Pattern1HitInterval);
        }

        Destroy(whirlwind);
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    /// <summary>
    /// 깃털 낙하 패턴 - 임의의 위치에 경고 후 깃털이 떨어지고 범위 내 플레이어에게 피해
    /// </summary>
    private IEnumerator Pattern2Coroutine()
    {
        List<GameObject> warnings = new List<GameObject>();
        List<Vector2> positions = new List<Vector2>();

        // 1. 랜덤한 위치에 경고 생성
        for (int i = 0; i < FeatherCount; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-Pattern2BoxSize.x / 2f, Pattern2BoxSize.x / 2f),
                Random.Range(-Pattern2BoxSize.y / 2f, Pattern2BoxSize.y / 2f)
            );

            Vector2 pos = (Vector2)Pattern2Center.position + offset;

            warnings.Add(Instantiate(WarningFeatherSpotPrefab, pos, Quaternion.identity));
            positions.Add(pos);
        }

        // 2. 대기 후 애니메이션, 사운드
        yield return new WaitForSeconds(Pattern2Delay);
        BossAnimator.Play("Boss_WingOpen");
        AudioSource.PlayClipAtPoint(RoarSound2, transform.position);

        // 3. 깃털 생성 및 데미지 판정
        for (int i = 0; i < positions.Count; i++)
        {
            Instantiate(FeatherEffectPrefab, positions[i], Quaternion.identity);
            DealDamageInCircle(positions[i], 1.5f, Pattern2DamagePercent);
            Destroy(warnings[i]);
        }

        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    /// <summary>
    /// 돌진 패턴 - 일정 시간 대기 후 목표 지점까지 빠르게 돌진하며 충돌 시 피해
    /// </summary>
    private IEnumerator Pattern3Coroutine()
    {
        hasChargedHit = false; // 충돌 여부 초기화
        // 1. 경고 생성
        GameObject warning = Instantiate(WarningRectPrefab, transform.position, Quaternion.identity);

        // 2. 대기 시간 후 애니메이션/사운드/이펙트
        yield return new WaitForSeconds(ChargeDelay);
        AudioSource.PlayClipAtPoint(ChargeSound, transform.position);
        GameObject aura = Instantiate(AuraEffect, transform.position, Quaternion.identity);

        // 3. 보스 방향 설정 및 위치 이동 준비
        transform.position = ChargeStartPos.position;
        Vector2 dir = (ChargeEndPos.position - ChargeStartPos.position).normalized;

        ChargeCollider.enabled = true; // 충돌 활성화
        BossRigidbody.velocity = dir * ChargeForce;

        yield return new WaitForSeconds(1.2f); // 돌진 지속 시간

        // 4. 돌진 종료 처리
        BossRigidbody.velocity = Vector2.zero;
        ChargeCollider.enabled = false;

        Destroy(warning);
        Destroy(aura);

        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCharging || hasChargedHit) return; // 돌진 중이 아니거나 이미 충돌했으면 무시

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Game.Data.PlayerData>();
            if (player != null)
            {
                player.TakeDamageByPercent(Pattern3DamagePercent / 100f);
                Debug.Log($"[Stage3BossFSM] 돌진으로 플레이어 {other.name}에게 {Pattern3DamagePercent}% 피해");
            }

            hasChargedHit = true; // 중복 충돌 방지
        }
    }

    /// <summary>
    /// 지정된 위치의 원형 범위 내에 있는 플레이어에게 퍼센트 데미지를 준다
    /// </summary>
    private void DealDamageInCircle(Vector2 center, float radius, float percent)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Game.Data.PlayerData>();
                if (player != null)
                {
                    player.TakeDamageByPercent(percent / 100f);
                    Debug.Log($"[Stage3BossFSM] 플레이어 {hit.name}에게 {percent}% 데미지");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 회오리 중심 위치가 설정되어 있으면 범위 기즈모를 그림
        if (Pattern1Origin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Pattern1Origin.position, Pattern1Range);
        }
        // 패턴2 깃털 낙하 박스 범위 표시
        if (Pattern2Center != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Pattern2Center.position;
            Vector3 size = new Vector3(Pattern2BoxSize.x, Pattern2BoxSize.y, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
