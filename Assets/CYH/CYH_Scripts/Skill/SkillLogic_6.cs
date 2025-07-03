using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_6 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;
    
    [SerializeField] private Vector2 _hitBoxSize = new Vector2(1,1);
    [SerializeField] private LayerMask _monsterLayer;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    //public PlayerController PlayerController { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 6 초기화");
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = 6;
    }

    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerTypeA_Copy>();
        SkillData = _data;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UseSkill(transform);
        }
    }

    public void UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        //if (_isCooldown) return;
        if (IsCooldown) return;
        Debug.Log($"IsCooldown: {IsCooldown}");

        Debug.Log("스킬6 사용");

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        AnimationPlay();

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        DetectMonster();
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        //if (_isCooldown) return;
        if (IsCooldown) return;
        Debug.Log($"IsCooldown: {IsCooldown}");

        Debug.Log("스킬3 사용");

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        AnimationPlay();

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        DetectMonster();
    }

    public void SkillRoutine()
    {
        OnAttackEnd();
    }

    public void OnAttackStart()
    {
        _isSkillUsed = true;
    }

    public void OnAttackEnd()
    {
        _isSkillUsed = false;
    }

    public void AnimationPlay()
    {
        _animator.SetTrigger("UseSkill_6");
        //PlayerController.Instance.SetTrigger("UseSkill_6");
    }

    private void DetectMonster()
    {
        Vector2 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, _hitBoxSize, 0f, _monsterLayer); ;

        foreach (var col in hits)
        {
            _hitMonsters.Add(col.gameObject);
            //Debug.Log($"{col.gameObject.name}");
        }
    }

    protected override void Damage(GameObject monster)
    {
        //float damage = _playerController.AttackPoint * (1.0f + 0.01f * SkillLevel);
        long damage = (long)(PlayerController.Instance.GetAttack() * (1.0f + 0.01f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");
    }

    private IEnumerator CooldownCoroutine()
    {
        //float remaining = _data.CoolTime;
        float remaining = SkillData.CoolTime;
        while (remaining > 0f)
        {
            //Debug.Log($"쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        //_isCooldown = false;
        IsCooldown = false;
        Debug.Log("쿨타임 종료");
    }

    private void OnDrawGizmos()
    {
        
    }
}
