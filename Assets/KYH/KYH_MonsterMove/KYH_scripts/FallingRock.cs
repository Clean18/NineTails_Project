using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FallingRock : MonoBehaviour
{
    [Header("데미지 설정")]
    [SerializeField] private float DamagePercent = 0.1f;     // 플레이어에게 줄 데미지 비율
    [SerializeField] private string TargetTag = "Player";    // 데미지를 둘 대상의 태그
    [SerializeField] private float DamageRadius = 1.5f;      // 피격 판정 범위( 원형 )

    [Header("낙석 정지 조건")]
    [SerializeField] private float StopThreshold = 0.3f;     // Y축 기준 워닝포인트 도달했을때의 좌표값 오차 허용 오차
    [SerializeField] private float DropForce = 30f;          // 낙하 시 적용할 힘의 세기

    [Header("참조 위치")]
    public Transform WarningPoint;                           // 낙하 대상 지점 ( BossMonsterSFM 에서 할당)

    [Header("사운드 설정")]
    [SerializeField] private AudioClip DropRockSound;                  // 낙석 충돌 사운드
    [SerializeField] private AudioMixerGroup sfxMixerGroup;           // Audio Mixer의 SFX 그룹
    private AudioSource audioSource;                                   // 사운드 재생용 AudioSource


    private Rigidbody2D rb;
    private bool hasDealtDamage = false;        // 데미지 판정이 이미 이루어졌는지 여부확인
    private bool hasLaunched = false;           // 낙하가 시작되었는지의 여부 확인
    [SerializeField] Animator DropRockAnimator;

    private void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();       // Rigidbody2D 컴포넌트 가져오기
        rb.gravityScale = 0f;                   // 중력 제거
                                                
        // AudioSource 생성 및 설정 (Mixer 연결 포함)
        audioSource = gameObject.AddComponent<AudioSource>();              // 런타임에 오디오 소스 추가
        audioSource.outputAudioMixerGroup = sfxMixerGroup;                // Mixer 그룹 연결 (SFX 그룹)
        audioSource.playOnAwake = false;                                  // 자동 재생 방지
        audioSource.loop = false;
        
        // 낙석 소리 재생 (PlayClipAtPoint 대신, Mixer 연동된 AudioSource로 재생)
        if (DropRockSound != null)
        {
            audioSource.PlayOneShot(DropRockSound);
        }

        if (WarningPoint != null)
        {
            // 경고 위치를 향해 방향 벡터 계산
            Vector2 direction = (WarningPoint.position - transform.position).normalized;

            // 해당 방향으로 즉시 낙하
            rb.AddForce(direction * DropForce, ForceMode2D.Impulse);
            hasLaunched = true;

            Debug.Log($"[FallingRock] AddForce로 낙하 시작 → 방향: {direction}, 힘: {DropForce}");
        }
        else
        {
            // 경고 지점이 설정되지않은 경우 디버그 로그를 출력함
            Debug.LogWarning("[FallingRock] WarningPoint가 할당되지 않음!");
        }

        // 혹시라도 도달하지 못한 경우 일정 시간 뒤 제거하는 함수
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        // 낙하 전이거나 이미 데미지를 준 경우 혹은 워닝 지점이 없다면 종료
        if (!hasLaunched || hasDealtDamage || WarningPoint == null) return;

        // 현재 위치와 경고 지점의 Y 축 값 비교
        float rockY = transform.position.y;
        float warningY = WarningPoint.position.y;
        float delta = rockY - warningY;

        Debug.Log($"[낙석 거리] RockY: {rockY}, WarningY: {warningY}, 차이: {delta}");

        // 도달 허용 오차 범위 이내이면 충돌로 판단하게 하는 조건문
        if (Mathf.Abs(delta) <= StopThreshold)
        {
            Debug.Log("[낙석] 워닝포인트에 도달! 정지 후 데미지 판정");

            rb.velocity = Vector2.zero;     // 속도 정지
            rb.isKinematic = true;          // 물리 계산 정지

            DealDamage();                   // 데미지 처리
            hasDealtDamage = true;          // 중복 판정 방지

            // 워닝 포인트 제거
            if (WarningPoint != null)
            {
                Destroy(WarningPoint.gameObject, 1.5f);
            }

            //낙석 오브젝트 제거하는 시간 ( 연출 시간에 따라 스크립트에서 수정가능
            Destroy(gameObject, 1.5f);
        }
    }

    // 데미지 판정 함수 ( 원형 범위 내에 있는 플레이어 컴포넌트 탐지 )
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
                    Debug.Log($" 낙석 데미지: {hit.name}에게 {DamagePercent * 100}% 피해");
                }
            }
        }
    }

    // 에디터에서 범위 시각화 하는 기즈모 ( 필요없으면 삭제해도 됨 )
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DamageRadius);
    }
}

