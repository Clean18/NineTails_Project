using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_0_HitBox : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private PolygonCollider2D _hitBox;
    private Animator _animator;

    [SerializeField] private int _slashCount = 0;


    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // 게임 스타트 -> 무기 collider 끔
        _hitBox.enabled = false;
        //_hitBox.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill();
        }
    }

    public void UseSkill()
    {
        _slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        Debug.Log("스킬사용 1타");
    }

    public void OnAttackStart()
    {
        _hitBox.enabled = true;
        //_hitBox.SetActive(true);
        Debug.Log("콜라이더 킴");
    }

    // 애니메이션이 끝났을 때 이벤트로 호출
    public void OnAttackEnd()
    {
        _hitBox.enabled = false;
        // _hitBox.SetActive(false);
        Debug.Log("콜라이더 끔");
        if (_slashCount == 2)
        {
            _slashCount = 0;
        }
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled)
            return;
        else
        {
            _animator.SetTrigger("UseSkill");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            if (_slashCount == 1)
            {
                Debug.Log("몬스터 맞음 + 1타");
                // 데미지 구현
            }
            else if (_slashCount == 2)
            {
                Debug.Log("몬스터 맞음 + 2타");
                // 데미지 구현
            }
            else
            {
                Debug.Log("몬스터 맞음");
            }
        }
    }

    /// <summary>
    /// 애니메이션 2타 타이밍에 애니메이션 이벤트로 호출
    /// </summary>
    private void SlashCountEvent()
    {
        _slashCount = 2;
        Debug.Log("스킬사용 2타");
    }
}
