using System.Collections;
using UnityEngine;

public class SkillLogic_6 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [Header("데미지 범위")]
    [SerializeField] private Vector2 _hitBoxSize = new Vector2(1, 1);
    [Header("데미지 범위 오프셋")]
    [SerializeField] private Vector2 _boxOffset = new Vector2(0, 0);
    [SerializeField] private LayerMask _monsterLayer;

    [Header("데미지 코루틴")]
    [SerializeField] private float _damageInterval = 0.1f;
    [SerializeField] private int _damageCount = 5;

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
        StartCoroutine(CooldownCoroutine());

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
        StartCoroutine(CooldownCoroutine());

        AnimationPlay();

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        DetectMonster();
    }

    public void SkillRoutine()
    {
        StartCoroutine(DamageRoutine());
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

    // 피격 몬스터 감지
    private void DetectMonster()
    {
        float offsetX = _boxOffset.x * Mathf.Sign(transform.localScale.x);
        Vector2 center = (Vector2)transform.position + new Vector2(offsetX, _boxOffset.y);

        Collider2D[] monsters = Physics2D.OverlapBoxAll(center, _hitBoxSize, 0f, _monsterLayer);
        foreach (var monster in monsters)
        {
            _hitMonsters.Add(monster.gameObject);
        }
    }

    protected override void Damage(GameObject monster)
    {
        float damage = _playerController.AttackPoint * (4.0f + 0.04f * SkillLevel);
        //long damage = (long)(PlayerController.Instance.GetAttack() * (4.0f + 0.04f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");
    }

    private IEnumerator DamageRoutine()
    {
        for (int i = 0; i < _damageCount; i++)
        {
            foreach (var monster in _hitMonsters)
                Damage(monster);
            yield return new WaitForSeconds(_damageInterval);
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        float remaining = SkillData.CoolTime;
        while (remaining > 0f)
        {
            yield return new WaitForSeconds(1f);
            remaining -= 1f;  
        }
        //_isCooldown = false;
        IsCooldown = false;
        Debug.Log("쿨타임 종료");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float offsetX = _boxOffset.x * Mathf.Sign(transform.localScale.x);
        Vector3 center = transform.position + new Vector3(offsetX, _boxOffset.y, 0f);
        Vector3 boxSize = new Vector3(_hitBoxSize.x, _hitBoxSize.y, 0.01f);

        Gizmos.DrawWireCube(center, boxSize);
    }
}