using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1스테이지 보스 FSM 클래스. BaseBossFSM을 상속하며,
/// Pattern1 (부채꼴 할퀴기), Pattern2 (낙석), Pattern3 (창귀 투사체) 구현.
/// </summary>
public class Stage1BossFSM : BaseBossFSM
{
    [Header("Pattern1 setting")]
    [SerializeField] private Animator BossAnimator;             // 보스 애니메이터
    [SerializeField] private GameObject AttackEffectPrefab;     // 휘두르기 이펙트 프리팹
    [SerializeField] private Transform AttackOrigin;            // 공격 중심 기준 위치 ( 예 : 오른팔 위치)
    [SerializeField] private float AttackRange = 5f;            // 부채꼴 공격 반경
    [SerializeField] private float AttackAngle = 90f;           // 부채꼴 공격 각도
    [SerializeField] private AudioClip SwingSound;              // 팔 휘두르는 사운드
    [SerializeField] private GameObject WarningRangeIndicator;  // 공격 경고 범위 ( 빨간 UI 와 같은것 )
    [SerializeField] private float Pattern1EffectDuration = 2f; // 이펙트와 경고의 유지시간.
    private GameObject CurrentWarningIndicator;

    [Header("Pattern2 Setting")]
    [SerializeField] private GameObject DropRockPrefab;         // 떨어지는 돌 프리팹
    [SerializeField] private GameObject WarningCirclePrefab;    // 바닥에 표시될 경고 원 프리팹
    [SerializeField] private Transform DropPosition;            // 돌이 떨어질 위치 (보스 앞 등)
    [SerializeField] private AudioClip RoarSound;               // 돌 떨어뜨리기 전 포효 사운드
    [SerializeField] private float Pattern2Delay = 3f;          // 경고 표시 후 돌이 떨어지는 시간
    [SerializeField] private float Pattern2EffectDuration = 2f; // 돌 이펙트가 유지되는 시간
    [SerializeField] private float DropRadius = 3f;             // 플레이어 위치 기준 낙석 출현 범위

    [Header("Pattern3 Setting")]
    [SerializeField] private GameObject SpearGhostPrefebs;   // 창귀 투사체 프리팹
    [SerializeField] private float SpearSpeed = 10f;            // 투사체 속도
    [SerializeField] private AudioClip SpearGhostSound;         // 발사 사운드
    [SerializeField] private GameObject WarningRectPrefab; // 경고용 직사각형 오브젝트
    [SerializeField] private float WarningDistance = 2f;   // 보스로부터 경고 오브젝트까지의 거리
    private List<GameObject> warningRects = new List<GameObject>();

    

    protected override void HandlePattern1()
    {

        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern1Coroutine());
            // 이후 Idle 상태로 돌아가야함
            // 코루틴 내부에서 완료후 상태전환 처리
        }
    }

    private IEnumerator Pattern1Coroutine()
    {
        // 1. 고정된 방향으로 설정 (왼쪽)
        Vector2 fixedDirection = Vector2.left;
        float fixedAngle = 135f;

        // 2. 경고 프리팹 인스턴스화 (왼쪽 방향)
        if (WarningRangeIndicator != null)
        {
            CurrentWarningIndicator = Instantiate(
                WarningRangeIndicator,
                AttackOrigin.position,
                Quaternion.Euler(0f, 0f, fixedAngle)
            );
        }

        // 3. 경고 후 대기
        yield return new WaitForSeconds(3f);

        // 4. 애니메이션 & 사운드
        BossAnimator.Play("Boss_Attack1");
        AudioSource.PlayClipAtPoint(SwingSound, transform.position);
        Debug.Log("패턴1 - 할퀴기 공격 시작");

        // 5. 이펙트 생성 (왼쪽 방향)
        if (AttackEffectPrefab != null)
        {
            GameObject fx = Instantiate(AttackEffectPrefab, AttackOrigin.position, Quaternion.Euler(0f, 0f, fixedAngle));
            Destroy(fx, Pattern1EffectDuration);
        }

        // 6. 부채꼴 범위 내 데미지 판정
        DealDamageInCone(fixedDirection);

        // 7. 이펙트 유지 시간 대기
        yield return new WaitForSeconds(Pattern1EffectDuration);

        // 8. 경고 제거
        if (CurrentWarningIndicator != null)
            Destroy(CurrentWarningIndicator);

        // 9. 상태 복귀
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    /// <summary>
    /// 부채꼴 범위 내에 있는 플레이어에게 데미지를 주는 함수.
    /// 중심점은 AttackOrigin.position, 방향은 forwardDirection 기준.
    /// </summary>
    /// <param name="forwardDirection">공격의 기준 방향 (보통 플레이어 방향)</param>
    private void DealDamageInCone(Vector2 forwardDirection)
    {
        // 1. 중심 위치(AttackOrigin.position)를 기준으로 원형 범위 내에 있는 모든 Collider2D를 가져온다.
        Collider2D[] hits = Physics2D.OverlapCircleAll(AttackOrigin.position, AttackRange);

        foreach (var hit in hits)
        {
            // 2. 태그가 "Player"인 대상만 공격 대상으로 고려
            if (!hit.CompareTag("Player")) continue;

            // 3. 대상까지의 방향 벡터 계산
            Vector2 dirToTarget = (hit.transform.position - AttackOrigin.position).normalized;

            // 4. 공격 방향(forwardDirection)과 대상 방향(dirToTarget) 사이의 각도 계산
            float angle = Vector2.Angle(forwardDirection, dirToTarget);

            // 5. 부채꼴 범위 안에 있을 경우에만
            if (angle <= AttackAngle / 2f)
            {
                // 6. PlayerData 스크립트가 붙어 있다면 데미지 처리
                var player = hit.GetComponent<Game.Data.PlayerData>();
                if (player != null)
                {
                    Debug.Log($"패턴1 - 플레이어 {hit.name}에게 40% 데미지");
                    player.TakeDamageByPercent(0.4f); // 체력의 40%를 데미지로 줌
                }
            }
        }
    }

    protected override void HandlePattern2()
    {
        // 
        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern2Coroutine());
        }
        // 이후 Idle 상태로 되돌아감
    }

    private IEnumerator Pattern2Coroutine()
    {
        // 1. 보스 애니메이션, 사운드
        BossAnimator.Play("Boss_Roar");
        AudioSource.PlayClipAtPoint(RoarSound, transform.position);


        // 2. 낙석 위치 랜덤 계산
        Vector2 Randomoffset = UnityEngine.Random.insideUnitCircle * DropRadius;
        // *. 낙석 위치를 플레이어 위치 주변으로 설정
        Vector3 DropPos = PlayerTransform.position + new Vector3(Randomoffset.x, Randomoffset.y, 0f);

        // Vector3 DropPos = DropPosition.position + new Vector3(Randomoffset.x, Randomoffset.y, 0f);

        // 3. 경고 범위 생성
        GameObject Warning = Instantiate(WarningCirclePrefab, DropPos, Quaternion.identity);

        // 4. 3초 대기

        yield return new WaitForSeconds(Pattern2Delay);

        // 5. 돌 생성 (위에서 낙하)
        GameObject rock = Instantiate(DropRockPrefab, DropPos + Vector3.up * 5f, Quaternion.identity);

        FallingRock rockScript = rock.GetComponent<FallingRock>();
        if (rockScript != null)
        {
            rockScript.WarningPoint = Warning.transform;
        }

        // 7. 마무리 대기
        yield return new WaitForSeconds(0.5f);

        // 8. 상태 전환
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    protected override void HandlePattern3()
    {
        if (BossPatternRoutine == null)
        {
            BossPatternRoutine = StartCoroutine(Pattern3Coroutine());
        }
    }

    private IEnumerator Pattern3Coroutine()
    {
        // 1. 기준 각도 고정 (왼쪽)
        float baseAngle = 180f;

        // 2. 경고 표시
        ShowWarningRects(baseAngle);

        // 3. 대기 연출
        BossAnimator.Play("Boss_EyeGlow");
        Debug.Log("보스 창귀발사 대기모션");
        yield return new WaitForSeconds(3f);

        // 4. 투사 모션
        BossAnimator.Play("Boss_Slash3Way");
        AudioSource.PlayClipAtPoint(SwingSound, transform.position);
        yield return new WaitForSeconds(0.4f);

        // 5. 3방향 투사체 발사
        float[] angleOffsets = { 0f, 35f, -35f };
        foreach (float offset in angleOffsets)
        {
            FireSpearGhost(baseAngle, offset);
        }

        AudioSource.PlayClipAtPoint(SpearGhostSound, transform.position);
        yield return new WaitForSeconds(1f);

        // 6. 경고 제거
        HideWarningRects();

        // 7. 상태 복귀
        TransitionToState(BossState.Idle);
        BossPatternRoutine = null;
    }

    /// <summary>
    /// 지정된 각도 방향으로 창귀(투사체)를 생성하고 발사한다.
    private void FireSpearGhost(float baseAngle, float angleOffset)
    {
        float finalAngle = baseAngle + angleOffset;
        Vector3 direction = Quaternion.Euler(0, 0, finalAngle) * Vector3.right;

        Vector3 spawnPosition = AttackOrigin.position + direction.normalized * WarningDistance;
        GameObject spear = Instantiate(SpearGhostPrefebs, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * SpearSpeed;
        }

        spear.transform.rotation = Quaternion.Euler(0, 0, finalAngle);
    }

    /// <summary>
    /// 3방향(정면, 위, 아래)으로 경고 사각형을 표시한다.
    /// </summary>
    private void ShowWarningRects(float baseAngle)
    {
        ShowOneWarning(baseAngle, 0f);    // 정면
        ShowOneWarning(baseAngle, 35f);   // 위쪽 방향
        ShowOneWarning(baseAngle, -35f);  // 아래쪽 방향
    }

    /// <summary>
    /// 하나의 경고 사각형을 특정 각도 방향으로 생성한다.
    /// </summary>
    /// <param name="angleOffset">오프셋 각도 (예: 0도, +35도, -35도)</param>
    private void ShowOneWarning(float baseAngle, float angleOffset)
    {
        // 최종 각도 계산
        float finalAngle = baseAngle + angleOffset;

        // 방향 벡터 계산
        Vector3 direction = Quaternion.Euler(0, 0, finalAngle) * Vector3.right;

        // 경고 사각형의 생성 위치 계산
        Vector3 pos = AttackOrigin.position + direction.normalized * WarningDistance;

        // 경고 사각형 생성
        GameObject warning = Instantiate(WarningRectPrefab, pos, Quaternion.Euler(0, 0, finalAngle));
        warningRects.Add(warning);
    }

    private void HideWarningRects()
    {
        for (int i = 0; i < warningRects.Count; i++)
        {
            if (warningRects[i] != null)
            {
                Destroy(warningRects[i]);
            }

        }
        warningRects.Clear();
    }
}
