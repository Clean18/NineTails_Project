using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillLogic_2 : SkillLogic, ISkill
{
    [Header("Projectile 프리팹")]
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Projectile 수")]
    [SerializeField] private int _objCount = 3;

    [Header("원 궤도 설정")]
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _objSpeed = 140f;
    [SerializeField] private Vector3 _centerOffset = new Vector3(-0.18f, 1.34f, 0);

    [Header("스킬 지속 시간")]
    [SerializeField] private float _spinDuration = 7f;

    private GameObject[] _projectile;
    private float _degree;
    //private bool _isCooldown;
    public bool _isSpinning;

    private Coroutine _spinRoutine;
    private Coroutine _durationRoutine;
    private Coroutine _cooldownRoutine;

    private WaitForSeconds _spinDurationWait;
    private WaitForSeconds _cooldownWait;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    private void Start()
    {
        _spinDurationWait = new WaitForSeconds(_spinDuration);
        _cooldownWait = new WaitForSeconds(1f);
        _centerOffset = new Vector3(-0.18f, 1.34f, 0);

        _projectile = new GameObject[_objCount];
        for (int i = 0; i < _objCount; i++)
        {
            _projectile[i] = Instantiate(_projectilePrefab, transform);
            // _projectilePrefab 비활성화
            _projectile[i].SetActive(false);
        }
    }

    public void SkillInit()
    {
        Debug.Log("스킬 2 초기화");
        IsCooldown = false;
        SkillLevel = 1;
        SlotIndex = 2;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        UseSkill(transform);
    //    }
    //}

    public void UseSkill(Transform attacker)
    {
        Debug.Log("스킬 2 UseSkill");
        // 쿨타임 체크
        if (IsCooldown || _isSpinning)
        {
            Debug.Log("스킬 2 쿨타임이거나 사용중입니다.");
            return;
        }

        // 스킬 사용
        Debug.Log("스킬_2 사용");
        _isSpinning = true;
        // _projectilePrefab 활성화
        SetProjectileActive(true);
        Debug.Log("프리팹 활성화");

        // 보호막 체력 설정
        //PlayerController.PlayerModel.Data.ShieldHp = (long)(PlayerController.PlayerModel.Data.MaxHp * (0.25f + 0.0025f * _skillLevel));
        PlayerController.Instance.TakeShield((long)(PlayerController.Instance.GetMaxHp() * (0.25f + 0.0025f * _skillLevel)));

        // 매 프레임 원 운동 갱신
        _spinRoutine = StartCoroutine(SpinCoroutine());
        Debug.Log("원운동 갱신");

        // 지속시간 체크 시작
        _durationRoutine = PlayerController.Instance.StartCoroutine(SpinDurationCoroutine());
        // 쿨타임 체크 시작
        IsCooldown = true;
        _cooldownRoutine = PlayerController.Instance.StartCoroutine(CooldownCoroutine());
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임 체크
        if (IsCooldown || _isSpinning) return;

        // 스킬 사용
        Debug.Log("스킬_2 사용");
        _isSpinning = true;
        // _projectilePrefab 활성화
        SetProjectileActive(true);

        // 매 프레임 원 운동 갱신
        _spinRoutine = PlayerController.Instance.StartCoroutine(SpinCoroutine());

        // 지속시간 체크 시작
        _durationRoutine = PlayerController.Instance.StartCoroutine(SpinDurationCoroutine());
        
        // 쿨타임 체크 시작
        IsCooldown = true;
        _cooldownRoutine = PlayerController.Instance.StartCoroutine(CooldownCoroutine());
    }

    // 원 운동 로직
    private void UpdateProjectiles()
    {
        float interval = 360f / _objCount;
        Vector3 center = transform.position + _centerOffset;

        for (int i = 0; i < _objCount; i++)
        {
            float angle = _degree + interval * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Sin(rad) * _radius,
                Mathf.Cos(rad) * _radius,
                0f
            );
            _projectile[i].transform.position = center + offset;
        }
    }

    #region Coroutine
    // 원 운동
    private IEnumerator SpinCoroutine()
    {
        //  _projectilePrefab 한 바퀴 회전에 걸리는 시간 : 360f / _objSpeed
        while (_isSpinning)
        {
            _degree = (_degree + Time.deltaTime * _objSpeed) % 360f;
            UpdateProjectiles();
            yield return null;
        }
    }

    // 지속 시간
    private IEnumerator SpinDurationCoroutine()
    {
        yield return _spinDurationWait;
        _isSpinning = false;
        if (_spinRoutine != null) PlayerController.Instance.StopCoroutine(_spinRoutine);
        Debug.Log("스킬 지속 시간 종료");

        // 스킬 지속 시간 종료 시 보호막 체력 = 0
        PlayerController.Instance.ClearShield();
        
        // _projectilePrefab 활성화
        SetProjectileActive(false);
    }

    // 쿨타임
    private IEnumerator CooldownCoroutine()
    {
        float remaining = SkillData.CoolTime;
        while (remaining > 0f)
        {
            //Debug.Log($"쿨타임 남음: {remaining}초");
            yield return _cooldownWait;
            remaining -= 1f;
        }
        IsCooldown = false;
        Debug.Log("쿨타임 종료");
    }

    // 피격 쿨타임
    private IEnumerator DamageCooldownCouroutine(GameObject monsterObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_hitMonsters.Contains(monsterObj))
        {
            _hitMonsters.Remove(monsterObj);
            Debug.Log($"{monsterObj.name}를 리스트에서 삭제");
        }
    }
    #endregion

    protected override void Damage(GameObject monsters)
    {
        long damage = (long)(PlayerController.Instance.GetAttack() * ((0.25f + 0.0025f * _skillLevel)));
        monsters?.GetComponent<IDamagable>().TakeDamage(damage);
        Debug.Log($"{monsters.name}에게 {damage}의 피해를 가했음");
    }

    #region _projectilePrefab, Event
    // _projectilePrefab 활성화/비활성화
    private void SetProjectileActive(bool isActive)
    {
        foreach (var t in _projectile)
            t.SetActive(isActive);
    }

    // _projectilePrefab와 충돌한 monster를 리스트에 추가 후 피격
    private void HandleCollision(Collider2D monster)
    {
        var monsterObj = monster.gameObject;
        if (!_hitMonsters.Contains(monsterObj))
        {
            _hitMonsters.Add(monsterObj);
            Debug.Log($"리스트에 {monsterObj.name} 추가");
            Damage(monsterObj);
            PlayerController.Instance.StartCoroutine(DamageCooldownCouroutine(monsterObj, 1f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position+_centerOffset, _radius);
    }

    private void OnEnable()
    {
        Skill_2_Projectile.Skill_2_Event += HandleCollision;
    }

    private void OnDisable()
    {
        Skill_2_Projectile.Skill_2_Event -= HandleCollision;
    }
    #endregion
}