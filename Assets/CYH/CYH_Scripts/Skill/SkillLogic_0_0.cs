using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_0_0 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private PolygonCollider2D _hitBox;
    [SerializeField] private List<GameObject> _hitMonsters = new List<GameObject>();
    [SerializeField] private int _slashCount = 0;
    [SerializeField] private int skillLevel = 0;

    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // 게임 스타트 -> _hitBox collider 끔
        _hitBox.enabled = false;
        //_hitBox.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill(transform);
        }
    }

    public void UseSkill(Transform attacker)
    {
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        _slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        Debug.Log("스킬사용 1타");
    }
    public void UseSkill(Transform attacker, Transform defender)
    {
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

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

        // 몬스터 TakeDamage 처리
        Damage();

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

        if (!other.CompareTag("Monster")) return;
        if (!_hitMonsters.Contains(other.gameObject))
        {
            _hitMonsters.Add(other.gameObject);
        }

        Debug.Log($"몬스터 맞음 + {_slashCount}타");
    }

    /// <summary>
    /// 애니메이션 2타 타이밍에 호출되는 이벤트 메서드입니다
    /// </summary>
    public void SlashCountEvent()
    {
        // 1타용 리스트 초기화
        _hitMonsters.Clear();

        _slashCount = 2;
        Debug.Log("스킬사용 2타");
        //OnAttackStart();
    }

    /// <summary>
    /// 각 타마다 _hitMonsters 리스트에 담긴 몬스터에게 한 번씩만 데미지 처리하는 메서드입니다.
    /// </summary>
    private void Damage()
    {
        float damage = _playerController.AttackPoint * (100f + skillLevel) / 100f;

        if(_slashCount == 1)
        {
            foreach (var monster in _hitMonsters)
            {
                monster.GetComponent<Monster_CYH>().TakeDamage(damage);
                Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음!");
            }
        }
        else if(_slashCount == 2)
        {
            foreach (var monster in _hitMonsters)
            {
                monster.GetComponent<Monster_CYH>().TakeDamage(damage*0.5f);
                Debug.Log($"{monster.name}에게 {damage * 0.5f}의 피해를 가했음!");
            }
        }
    }
}