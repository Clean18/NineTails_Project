using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_4 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private float _radius = 2f;

    [Header("랜덤 대상 개수")]
    [SerializeField] private int _randomTargetCount = 6;

    [SerializeField] List<GameObject> _randomMonsters = new List<GameObject>();

    [Header("데미지 코루틴 (초)")]
    [SerializeField] private float _damageInterval = 0.2f;
    [Header("이펙트 활성 지속 시간 (초)")]
    [SerializeField] private float _effectDuration = 0.1f;
    [Header("데미지 이펙트 프리팹")]
    [SerializeField] private GameObject _damageEffectPrefab;
    [Header("이펙트 Y 오프셋")]
    [SerializeField] private float _effectYOffset = 0.5f;

    public PlayerController PlayerController { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public bool IsCooldown { get; set; }
    public int SkillLevel { get; set; }


    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerTypeA_Copy>();
        _animator = GetComponent<Animator>();
        SkillData = _data;
        IsCooldown = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
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

        Debug.Log("스킬4 사용");

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        StartCoroutine(CooldownCoroutine());

        //AnimationPlay();

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        _randomMonsters.Clear();

        OnAttackStart();
        DetectMonster();
        SkillRoutine();
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        //if (_isCooldown) return;
        if (IsCooldown) return;
        Debug.Log($"IsCooldown: {IsCooldown}");

        Debug.Log("스킬4 사용");

        // 쿨타임 체크 시작
        //_isCooldown = true;
        IsCooldown = true;
        StartCoroutine(CooldownCoroutine());

        //AnimationPlay();

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        _randomMonsters.Clear();

        OnAttackStart();
        DetectMonster();
        SkillRoutine();
    }

    public void SkillRoutine()
    {
        RandomDamage();
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
        _animator.SetTrigger("UseSkill_4");
        //PlayerController.Instance.SetTrigger("UseSkill_3");
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

    // 피격 몬스터 목록으로 새 랜덤 목록 생성
    private void RandomDamage()
    {
        if (_hitMonsters.Count == 0) return;

        if (_hitMonsters.Count <= _randomTargetCount)
        {
            Debug.Log("전부 넣기");
            _randomMonsters.AddRange(_hitMonsters);

        }
        else
        {
            _randomMonsters = new List<GameObject>(_hitMonsters);

            // _randomTargetCount번 뽑고 뽑을 때마다 제거 -> 중복 방지
            for (int i = 0; i < _randomTargetCount; i++)
            {
                int index = Random.Range(0, _randomMonsters.Count);
                _randomMonsters.Add(_randomMonsters[index]);
                _randomMonsters.RemoveAt(index);
            }
        }

        // 랜덤 리스트의 모든 몬스터 데미지 가함
        foreach (var monster in _randomMonsters)
        {
            Debug.Log($"_randomMonsters : {monster.name}");
            Damage(monster);
        }
    }

    protected override void Damage(GameObject monster)
    {
        float damage = _playerController.AttackPoint * (1.0f + 0.01f * SkillLevel);
        //float damage = PlayerController.PlayerModel.Data.Attack * (1.0f + 0.01f * SkillLevel);
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public void SkillInit()
    {
        Debug.Log("스킬 4 초기화");
    }
}

