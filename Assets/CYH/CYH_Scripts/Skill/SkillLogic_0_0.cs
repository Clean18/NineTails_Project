using Unity.VisualScripting;
using UnityEngine;

public class SkillLogic_0_0 : MonoBehaviour
{
    ActiveSkillData _data;
    
    [SerializeField] private Collider2D SwordCollider;
    [SerializeField] Animator WeaponAnimator;

    [SerializeField] private bool _isAttack = false;

    private void Start()
    {
        //swordCollider.enabled = false;
    }
  
    private void Update()
    {
        AnimationPlay();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill();
        }

        AnimatorStateInfo stateInfo = WeaponAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Skill_0") && stateInfo.normalizedTime >= 1.0f)
        {
            WeaponAnimator.SetBool("isAttack", false);
        }
    }

    public void UseSkill()
    {
        _isAttack = true;
        Debug.Log("스킬사용");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("트리거");
        //if (!swordCollider.enabled) return;
        if (col.CompareTag("Monster"))
        {
            Debug.Log("공격");
            //WeaponAnimator.SetBool("isAttack", false);
        }
    }

    public void AnimationPlay()
    {
        if (!_isAttack)
            return;
        else
        {
            SwordCollider.enabled = true;
            WeaponAnimator.SetBool("isAttack", true);
            _isAttack = false;
        }
    }
}
