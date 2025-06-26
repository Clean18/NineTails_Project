using UnityEngine;
using System.Collections;

public class SkillLogic_0 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData data;
    [SerializeField] private PlayerControllerTypeA_Copy playerController;

    [SerializeField] Transform rayPos;
    [SerializeField] float attackRange = 1f;
    [SerializeField] private bool isRayActive = false;

    [SerializeField] float slashTime;
    private int slashCount = 0;

    [SerializeField][Range(1f, 100f)] float rotateSpeed = 50f;
    [SerializeField] private bool isRotating = false;
    private float angle = 0f;

    private void EnableRaycast() => isRayActive = true;
    private void DisableRaycast() => isRayActive = false;

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
        rayPos.localRotation = Quaternion.identity;
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        EnableRaycast();
        StartRotation();
        slashCount = 1;
        Debug.Log("스킬사용 1타");

        // isRotating -> false 될 때까지 매 프레임 RotatePos(), DetectMonster() 실행
        while (isRotating)
        {
            RotatePos();
            DetectMonster();
            yield return null;
        }
    }

    private void DetectMonster()
    {
        if (!isRayActive)
            return;

        RaycastHit2D hit = Physics2D.Raycast(rayPos.position, rayPos.up, attackRange);
        Debug.DrawRay(rayPos.position, rayPos.up * attackRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Monster"))
        {
            Debug.Log("몬스터 감지");
        }
    }

    private void StartRotation()
    {
        if (isRotating) return;
        isRotating = true;
        angle = 0f;
    }

    private void RotatePos()
    {
        if (!isRotating)
            return;

        float rotationAngle = Time.deltaTime * rotateSpeed*10;

        if (angle + rotationAngle >= 180f)
        {
            float remain = 180f - angle;
            rayPos.transform.Rotate(0, 0, remain, Space.Self);
            isRotating = false;
            OnRotationComplete();
        }
        else
        {
            rayPos.transform.Rotate(0, 0, rotationAngle, Space.Self);
            angle += rotationAngle;
        }
    }

    private void OnRotationComplete()
    {
        Debug.Log("180 회전 완료");
        DisableRaycast(); 
    }
}