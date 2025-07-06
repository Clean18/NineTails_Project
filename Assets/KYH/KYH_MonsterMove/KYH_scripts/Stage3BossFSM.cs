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
    [SerializeField] private AudioClip CycloneSound;
    [SerializeField] private float Pattern1Duration = 3f;           // 회오리 공격 지속 시간
    [SerializeField] private float Pattern1HitInterval = 0.5f;      // 회오리 데미지 판정 간격
    [SerializeField] private float Pattern1DamagePercent = 0.05f;      // 회오리 1회 데미지 비율(%)
    [SerializeField] private float Pattern1Range = 3f;              // 회오리 공격 범위 (원형 범위)
    [SerializeField] private Transform Pattern1Origin;              // 회오리 중심 위치 (보스 중심 또는 지정 지점)

    [Header("Pattern2 - 깃털 낙하")]
    [SerializeField] private GameObject FeatherProjectilePrefab;                // 실제 깃털 오브젝트 프리팹
    [SerializeField] private GameObject WarningFeatherSpotPrefab;               // 깃털 경고 프리팹
  //  [SerializeField] private GameObject FeatherEffectPrefab;                    // 깃털 이펙트 프리팹
    [SerializeField] private AudioClip RoarSound2;                              // 깃털 공격 전 사운드
    [SerializeField] private float Pattern2Delay = 2f;                          // 경고 후 깃털 낙하까지 대기 시간
    [SerializeField] private float Pattern2DamagePercent = 0.05f;                  // 낙하한 깃털 데미지 비율(%)
    [SerializeField] private int FeatherCount = 6;                              // 깃털 낙하 수
    [SerializeField] private float FeatherDamageRadius = 1.5f;                  // 깃털 공격의 데미지 판정 원의 반지름
    [SerializeField] private Vector2 Pattern2BoxSize = new Vector2(6f, 4f);     // (너비, 높이)
    [SerializeField] private Transform Pattern2Center;                          // 낙하 박스 중심 위치

    [Header("Pattern3 - 돌진 공격")]
    [SerializeField] private GameObject WarningRectPrefab;          // 돌진 경고 직사각형 프리팹
    [SerializeField] private GameObject AuraEffect;                 // 돌진 시 오라 이펙트
    [SerializeField] private AudioClip RoarSound3;
    [SerializeField] private AudioClip ChargeSound;                 // 돌진 사운드
    [SerializeField] private float ChargeDelay = 3f;                // 돌진 전 대기 시간
    [SerializeField] private float Pattern3DamagePercent = 0.7f;     // 돌진 충돌 시 데미지 비율(%)
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
        Debug.Log("회오리 공격 패턴 실행됨");
        // 1. 경고 원 생성
        GameObject warning = Instantiate(WarningCirclePrefab, Pattern1Origin.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);

        // 2. 경고 제거 및 애니메이션+사운드 재생
        Destroy(warning);
        // BossAnimator.Play("Boss_WingOpen");
        PlaySound(RoarSound1);

        // 3. 회오리 이펙트 생성
        GameObject whirlwind = Instantiate(WhirlwindEffect, Pattern1Origin.position, Quaternion.identity);
        PlaySound(CycloneSound);
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

        // BossAnimator.Play("Bird_Idle_2");
    }

    /// <summary>
    /// 깃털 낙하 패턴 - 임의의 위치에 경고 후 깃털이 떨어지고 범위 내 플레이어에게 피해
    /// </summary>
    private IEnumerator Pattern2Coroutine()
    {
        Debug.Log("깃털 낙하 패턴 실행됨");
        List<GameObject> warnings = new List<GameObject>();
        List<Vector2> positions = new List<Vector2>();

        // 1. 경고 위치 랜덤 생성
        for (int i = 0; i < FeatherCount; i++)
        {
            Vector2 offset = new Vector2(
                Random.Range(-Pattern2BoxSize.x / 2f, Pattern2BoxSize.x / 2f),
                Random.Range(-Pattern2BoxSize.y / 2f, Pattern2BoxSize.y / 2f)
            );

            Vector2 pos = (Vector2)Pattern2Center.position + offset;

            GameObject warning = Instantiate(WarningFeatherSpotPrefab, pos, Quaternion.identity);
            warnings.Add(warning);
            positions.Add(pos);
        }

        // 2. 경고 대기 후 연출
        yield return new WaitForSeconds(Pattern2Delay);
        // BossAnimator.Play("Boss_WingOpen");
        PlaySound(RoarSound2);

        // 3. 깃털 생성
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 spawnPos = positions[i] + new Vector2(0f, 3f); // 위에서 낙하

            GameObject feather = Instantiate(FeatherProjectilePrefab, spawnPos, Quaternion.identity);

            // FallingFeather 스크립트 설정
            FallingFeather featherScript = feather.GetComponent<FallingFeather>();
            if (featherScript != null)
            {
                featherScript.WarningPoint = warnings[i].transform;
            }
            else
            {
                Debug.LogWarning("[Stage3BossFSM] FallingFeather 컴포넌트가 존재하지 않음!");
            }

            // 시각적 이펙트 생성
           // if (FeatherEffectPrefab != null)
           // {
           //     Instantiate(FeatherEffectPrefab, positions[i], Quaternion.identity);
           // }
        }

        // 4. FSM 상태 전환
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;

        // BossAnimator.Play("Bird_Idle_2");
    }

    /// <summary>
    /// 돌진 패턴 - 일정 시간 대기 후 목표 지점까지 빠르게 돌진하며 충돌 시 피해
    /// </summary>
    private IEnumerator Pattern3Coroutine()
    {
        Debug.Log("[보스] 몸통 박치기 패턴 실행됨");

        hasChargedHit = false;
        isCharging = false;
        PlaySound(RoarSound3);
        // 1. 경고 프리팹 생성
        GameObject warning = Instantiate(WarningRectPrefab);

        // 2. 대기 후 애니메이션, 사운드
        yield return new WaitForSeconds(ChargeDelay);
        PlaySound(ChargeSound);
        GameObject aura = Instantiate(AuraEffect, transform.position, Quaternion.identity);

        // 3. 부드럽게 돌진 시작 위치로 이동
        float t1 = 0f;
        float preChargeDuration = 2f;
        Vector3 from1 = transform.position;
        Vector3 to1 = ChargeStartPos.position;
        Vector3 chargeEndPos = ChargeEndPos.position;

        Debug.Log("박치기 : 시작 위치로");
        while (t1 < preChargeDuration)
        {
            t1 += Time.deltaTime;
            transform.position = Vector3.Lerp(from1, to1, t1 / preChargeDuration);
            yield return null;
        }

        // 4. 돌진 시작
        Debug.Log("박치기 : 돌진 시작");
        BossAnimator.Play("Bird_BodyAttack");
        Vector2 dir = (chargeEndPos - to1).normalized;

        isCharging = true;
        //ChargeCollider.enabled = true;
        BossRigidbody.velocity = dir * ChargeForce;

        //yield return new WaitForSeconds(1f); // 돌진 지속
        while (Vector3.Distance(transform.position, chargeEndPos) > 0.1f)
        {
            yield return null;
        }

        // 5. 돌진 종료
        Debug.Log("박치기 : 돌진 종료");
        BossRigidbody.velocity = Vector2.zero;
        //ChargeCollider.enabled = false;
        isCharging = false;

        // 6. 사라지는 연출 (Sprite만 꺼서 물리 영향 방지)
        _sprite.enabled = false;
        GetComponentInChildren<Collider2D>().enabled = false;
        

        Destroy(warning);
        Destroy(aura);
        yield return new WaitForSeconds(1f);


        BossAnimator.Play("Bird_Idle_2");

        // 7. 위에서 천천히 내려오는 연출
        Vector3 reappearStartPos = originalPosition + new Vector3(0f, 10f, 0f);
        transform.position = reappearStartPos;

        _sprite.enabled = true;
        GetComponentInChildren<Collider2D>().enabled = true;

        //BossRigidbody.isKinematic = true; // Lerp용
        float t2 = 0f;
        float fallDuration = 1.5f;

        while (t2 < fallDuration)
        {
            t2 += Time.deltaTime;
            transform.position = Vector3.Lerp(reappearStartPos, originalPosition, t2 / fallDuration);
            yield return null;
        }

        //BossRigidbody.isKinematic = false;

        // 8. 상태 전환
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;

        // BossAnimator.Play("Bird_Idle_2");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCharging || hasChargedHit)
        {
            Debug.Log("헤드부트");
            return;
        }


        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage((long)(PlayerController.Instance.GetMaxHp() * Pattern3DamagePercent));
            Debug.Log($"[Stage3BossFSM] 돌진으로 플레이어에게 {Pattern3DamagePercent * 100}% 피해");

            hasChargedHit = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging || hasChargedHit)
        {
            Debug.Log("헤드부트");
            return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage((long)(PlayerController.Instance.GetMaxHp() * Pattern3DamagePercent));
            Debug.Log($"[Stage3BossFSM] 돌진으로 플레이어에게 {Pattern3DamagePercent * 100}% 피해");

            hasChargedHit = true;
        }
    }

    /// <summary>
    /// 지정된 위치의 원형 범위 내에 있는 플레이어에게 퍼센트 데미지를 준다
    /// </summary>
    private void DealDamageInCircle(Vector2 center, float radius, float damagePercent)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, radius, _playerLayer);

        if (hit != null)
        {
            PlayerController.Instance.TakeDamage((long)(PlayerController.Instance.GetMaxHp() * damagePercent));
            Debug.Log($"[Stage3BossFSM] 플레이어에게 {damagePercent * 100}% 데미지");
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

        Debug.Log("보스 3 사망 다음 씬으로 이동");
        SceneChangeManager.Instance.LoadNextScene();
    }
}
