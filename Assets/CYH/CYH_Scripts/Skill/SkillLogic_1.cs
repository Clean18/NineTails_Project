using UnityEngine;

public class SkillLogic_1 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private Animator animator;

    private CapsuleCollider2D SwordCollider;


    private void Awake()
    {
        SwordCollider = GetComponent<CapsuleCollider2D>();
        SwordCollider.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill();
        }
    }
    public void UseSkill()
    {
        OnAttackStart();
        Debug.Log("스킬사용");
        AnimationPlay();
    }

    public void OnAttackStart()
    {
        SwordCollider.enabled = true;
        Debug.Log("콜라이더 킴");
    }

    public void OnAttackEnd()
    {
        SwordCollider.enabled = false;
        Debug.Log("콜라이더 끔");
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

    private void OnEnable()
    {
        SkillEvent.OnAAnimationEnd += OnAttackEnd;
    }

    private void OnDisable()
    {
        SkillEvent.OnAAnimationEnd -= OnAttackEnd;
    }
}