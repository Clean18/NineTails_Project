using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillLogic_0 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData data;
    [SerializeField] private PlayerControllerTypeA_Copy playerController;

    [SerializeField] Transform rayPos;
    [SerializeField] float attackRange = 1f;
    [SerializeField] private bool isActive = false;

    [SerializeField] float slashTime;
    private int slashCount = 0;

    [SerializeField][Range(1f, 100f)] float rotateSpeed = 50f;
    private float angle = 0f;
    [SerializeField] private bool isRotating = false;

    public void EnableRaycast() => isActive = true;
    public void DisableRaycast() => isActive = false;

    private void Start()
    {
        playerController.facingDir = -1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseSkill();
        }
    }

    public void UseSkill()
    {
        // 코루틴으로 1타 -> 2타 실행
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        EnableRaycast();

        for (int hitNum = 1; hitNum <= 2; hitNum++)
        {
            // 회전값 항상 초기화
            rayPos.localRotation = Quaternion.identity;
            angle = 0f;

            // 2타일 때만 Y축 180 회전 (좌우 뒤집기)
            if (hitNum == 2)
            {
                rayPos.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            // slashCount 세팅
            slashCount = hitNum;
            Debug.Log($"스킬사용 {slashCount}타");

            // 회전 시작
            StartRotation();

            // 회전이 끝날 때까지 매 프레임 회전 & 몬스터 감지
            while (isRotating)
            {
                RotatePos();
                DetectMonster();
                yield return null;
            }
        }
        // 2타 끝나면 Raycast 끄기
        DisableRaycast();
    }

    private void DetectMonster()
    {
        if (!isActive) return;

        RaycastHit2D hit = Physics2D.Raycast(rayPos.position, rayPos.up, attackRange);
        Debug.DrawRay(rayPos.position, rayPos.up * attackRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Monster"))
            Debug.Log("몬스터 감지!");
    }

    public void StartRotation()
    {
        if (isRotating) return;
        isRotating = true;
    }

    private void RotatePos()
    {
        if (!isRotating) return;

        float rotationAngle = Time.deltaTime * rotateSpeed * 10;
        if (angle + rotationAngle >= 180f)
        {
            float remain = 180f - angle;
            rayPos.Rotate(0, 0, remain, Space.Self);
            isRotating = false;
            Debug.Log("180° 회전 완료");
        }
        else
        {
            rayPos.Rotate(0, 0, rotationAngle, Space.Self);
            angle += rotationAngle;
        }
    }
}