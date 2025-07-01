using System.Collections;
using UnityEngine;

public class SkillLogic_3 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private float _radius = 2f;
    [SerializeField] GameObject _highestMonster;

    public PlayerController PlayerController { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public bool IsCooldown { get; set; }
    public int SkillLevel { get; set; }


    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerTypeA_Copy>();
        SkillData = _data;
        IsCooldown = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill(transform);
        }
    }

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
        Damage(_highestMonster);
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
        OnAttackEnd();
    }

    public void OnAttackStart()
    {
        _isSkillUsed = true;
    }

    public void OnAttackEnd()
    {
        // 몬스터 TakeDamage 처리
        Damage(_highestMonster);
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

            // 1) MonsterFSM 타입인지 체크
            if (monster.TryGetComponent<MonsterFSM>(out var mM))
                hp = mM.CurrentHp;

            // 2) RangeMonsterFSM 타입인지 체크
            else if (monster.TryGetComponent<RangeMonsterFSM>(out var mR))
                hp = mR.CurrentHp;

            // 3) BaseBossFSM 타입인지 체크
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

        Debug.Log($"_highestMonster : {_highestMonster.name}");

        return _highestMonster;
    }

    protected override void Damage(GameObject monster)
    {
        Debug.Log("Damage");
        float damage = _playerController.AttackPoint * (1.0f + 0.01f * SkillLevel);
        //float damage = PlayerController.PlayerModel.Data.Attack * (1.0f + 0.01f * SkillLevel);
        GetHighestHpMonster()?.GetComponent<IDamagable>().TakeDamage((long)damage);

        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
