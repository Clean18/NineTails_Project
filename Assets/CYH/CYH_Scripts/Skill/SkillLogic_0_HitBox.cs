using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_0_HitBox : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private PolygonCollider2D _hitBox;
    
    [SerializeField] private int _slashCount = 0;

    public PlayerController PlayerController { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public bool IsCooldown { get; set; }
    public int SkillLevel { get; set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // 게임 스타트 -> _hitBox collider 끔
        _hitBox.enabled = false;
        SkillData = _data;
        IsCooldown = false;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        UseSkill(transform);
    //    }
    //}

    public void UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        //if (_isCooldown) return;
        if (IsCooldown) return;

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        _slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        //Debug.Log("스킬사용 1타");
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        //if (_isCooldown) return;
        if (IsCooldown) return;

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        _slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        //Debug.Log("스킬사용 1타");
    }

    public void OnAttackStart()
    {
        // OnTrigger 플래그
        _isSkillUsed = true;

        _hitBox.enabled = true;
        //Debug.Log("콜라이더 킴");
    }

    // 애니메이션이 끝났을 때 이벤트로 호출
    public void OnAttackEnd()
    {
        _hitBox.enabled = false;
        //Debug.Log("콜라이더 끔");

        // 몬스터 TakeDamage 처리
        Damage();

        if (_slashCount == 2)
        {
            _slashCount = 0;
        }

        // OnTrigger 플래그
        _isSkillUsed = false;
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled)
            return;
        else
        {
            _animator.SetTrigger("UseSkill_0");
        }
    }

    //// 애니메이션이 끝났을 때 이벤트로 호출
    public void SlashCountEvent()
    {
        // 1타용 리스트 초기화
        _hitMonsters.Clear();

        _slashCount = 2;
        //Debug.Log("스킬사용 2타");
        //OnAttackStart();
    }

    // 각 타마다 _hitMonsters 리스트에 담긴 몬스터에게 한 번씩만 데미지 처리
    protected override void Damage()
    {
        //float damage = _playerController.AttackPoint * (100f + _skillLevel) / 100f;
        float damage = PlayerController.PlayerModel.Data.Attack * (100f + _skillLevel) / 100f;

        if(_slashCount == 1)
        {
            foreach (var monster in _hitMonsters)
            {
                //monster.GetComponent<Monster_CYH>().TakeDamage(damage);
                monster?.GetComponent<IDamagable>().TakeDamage((long)(damage));
                Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
            }
        }
        else if(_slashCount == 2)
        {
            foreach (var monster in _hitMonsters)
            {
                //monster.GetComponent<Monster_CYH>().TakeDamage(damage*0.5f);
                monster?.GetComponent<IDamagable>().TakeDamage((long)(damage * 0.5f));
                Debug.Log($"{monster.name}에게 {(int)(damage * 0.5f)}의 피해를 가했음");
                //Debug.Log($"{monster.name}에게 {(damage * 0.5f)}의 피해를 가했음");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isSkillUsed) return;

        if (!other.CompareTag("Monster")) return;
        if (!_hitMonsters.Contains(other.gameObject))
        {
            _hitMonsters.Add(other.gameObject);
        }
        //Debug.Log($"Skill_0 : 몬스터 맞음 : {_slashCount}타");
    }

    // 쿨타임 코루틴
    private IEnumerator CooldownCoroutine()
    {
        //float remaining = _data.CoolTime;
        float remaining = SkillData.CoolTime;
        while (remaining > 0f)
        {
            Debug.Log($"쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        //_isCooldown = false;
        IsCooldown = false;
        Debug.Log("쿨타임 종료");
    }
}