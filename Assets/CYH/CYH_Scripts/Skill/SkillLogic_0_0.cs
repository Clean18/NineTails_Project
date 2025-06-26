using Unity.VisualScripting;
using UnityEngine;

public class SkillLogic_0_0 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData data;
    [SerializeField] private PlayerControllerTypeA_Copy playerController;

    private CapsuleCollider2D SwordCollider;
    private Animator animator;

    private int slashCount = 0;


    private void Awake()
    {
        SwordCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        
        // 게임 스타트 -> 무기 collider 끔
        SwordCollider.enabled = false;
    }

    private void Start()
    {
        playerController.facingDir = -1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Flip();
        }

        //Debug.Log($"facingDir : {playerController.facingDir}");
    }

    public void UseSkill()
    {
        slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        Debug.Log("스킬사용 1타");
    }

    public void OnAttackStart()
    {
        SwordCollider.enabled = true;
        Debug.Log("콜라이더 킴");
    }

    // 애니메이션이 끝났을 때 이벤트로 호출
    public void OnAttackEnd()
    {
        SwordCollider.enabled = false;
        Debug.Log("콜라이더 끔");

        if(slashCount == 1)
        {
            slashCount = 2;
            Flip();
            OnAttackStart();
            AnimationPlay();
            Debug.Log("스킬사용 2타");
        }
        else
        {
            slashCount = 0;
        }
    }

    public void AnimationPlay()
    {
        if (!SwordCollider.enabled)
            return;
        else
        {
            animator.SetTrigger("UseSkill");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("몬스터 맞음");
            // 데미지 로직 호출
        }
    }

    private void Flip()
    {
        Debug.Log("Filp");
        playerController.facingDir *= -1;
        gameObject.transform.parent.localScale = new Vector3(playerController.facingDir, 1, 1);
        Debug.Log($"facingDir : {playerController.facingDir}");
    }
}