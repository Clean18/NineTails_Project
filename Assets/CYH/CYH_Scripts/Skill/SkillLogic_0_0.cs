using Unity.VisualScripting;
using UnityEngine;

public class SkillLogic_0_0 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;

    private CapsuleCollider2D SwordCollider;
    private Animator Animator;


    private void Awake()
    {
        SwordCollider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        
        // 게임 스타트 -> 무기 collider 끔
        SwordCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill();
            AnimationPlay();
        }
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
    }

    public void UseSkill()
    {
        OnAttackStart();
        Debug.Log("스킬사용");
    }

    public void AnimationPlay()
    {
        if (!SwordCollider.enabled)
            return;
        else
        {
            Animator.SetTrigger("UseSkill");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("몬스터 히트!");
            // 데미지 로직 호출
        }
    }
}