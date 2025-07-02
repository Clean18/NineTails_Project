using System.Collections;
using UnityEngine;

public class SkillLogic_3 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private float _radius = 2f;
    [SerializeField] GameObject _highestMonster;
    [Header("데미지 코루틴 (초)")]
    [SerializeField] private float _damageInterval = 0.2f;
    [Header("이펙트 활성 지속 시간 (초)")]
    [SerializeField] private float _effectDuration = 0.1f;
    [Header("데미지 이펙트 프리팹")]
    [SerializeField] private GameObject _damageEffectPrefab;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }
    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerTypeA_Copy>();
        SkillData = _data;
        IsCooldown = false;
    }

    public void SkillInit()
    {
        Debug.Log("스킬 3 초기화");
        IsCooldown = false;
        SkillLevel = 1;
        SlotIndex = 3;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha3))
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

        Debug.Log("스킬3 사용");
        OnAttackStart();
        DetectMonster();
        GetHighestHpMonster();
        if (_highestMonster != null)
            StartCoroutine(DamageCoroutine(_highestMonster));

        OnAttackEnd();
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

        Debug.Log("스킬3 사용");
        OnAttackStart();
        DetectMonster();
        GetHighestHpMonster();

        if (_highestMonster != null)
            StartCoroutine(DamageCoroutine(_highestMonster));

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

    private void DetectMonster()
    {
        Vector3 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _radius);

        foreach (var col in hits)
        {
            if (col.CompareTag("Monster"))
            {
                _hitMonsters.Add(col.gameObject);
                Debug.Log($"{col.gameObject.name} 감지");
            }
        }
    }

    // 피격 몬스터 리스트 중 가장 체력이 높은 몬스터 반환
    private GameObject GetHighestHpMonster()
    {
        Debug.Log($"감지된 몬스터 수: {_hitMonsters.Count}");

        _highestMonster = null;
        float highestHp = float.MinValue;

        foreach (var monster in _hitMonsters)
        {
            float hp = 0f;

            // 1) <MonsterFSM>인지 체크
            if (monster.TryGetComponent<MonsterFSM>(out var mM))
                hp = mM.CurrentHp;

            // 2) <RangeMonsterFSM>인지 체크
            else if (monster.TryGetComponent<RangeMonsterFSM>(out var mR))
                hp = mR.CurrentHp;

            // 3) <BaseBossFSM>인지 체크
            else if (monster.TryGetComponent<BaseBossFSM>(out var mB))
                hp = mB.CurrentHealth;

            // 위 세 타입 중 하나라도 만족할 때
            if (hp > highestHp)
            {
                highestHp = hp;
                _highestMonster = monster;
            }
        }

        if (_highestMonster != null)
            Debug.Log($"최고 체력 몬스터: {_highestMonster.name} : HP {highestHp}");
        else
            Debug.Log("감지된 몬스터 x");

        return _highestMonster;
    }

    protected override void Damage(GameObject monster)
    {
        //float damage = _playerController.AttackPoint * (1.0f + 0.01f * SkillLevel);
        long damage = (long)(PlayerController.Instance.GetAttack() * ((1.0f + 0.01f * SkillLevel)));
        monster?.GetComponent<IDamagable>().TakeDamage(damage);
        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");

        // 현재 :몬스터 하위에 생성 x
        // 피격 몬스터 하위에 이펙트 생성
        if (_damageEffectPrefab != null && monster != null)
        {
            Instantiate(_damageEffectPrefab, monster.transform.position, Quaternion.identity, monster.transform);
        }
    }

    #region Coroutine
    private IEnumerator DamageCoroutine(GameObject monster)
    {
        for (int i = 1; i < 6; i++)
        {
            Debug.Log($"데미지 {i}번");
            Damage(_highestMonster);
            yield return new WaitForSeconds(_damageInterval);
        }
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
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
