using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_2 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [Header("Projectile 프리팹")]
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Projectile 수")]
    [SerializeField] private int _objCount = 3;

    [Header("원 궤도 설정")]
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _objSpeed = 140f;

    [Header("스킬 지속 시간")]
    [SerializeField] private float _spinDuration = 7f;

    [SerializeField] private int _skillLevel = 0;
    [SerializeField] private List<GameObject> _hitMonsters = new List<GameObject>();

    private GameObject[] _targets;
    private float _degree;
    private bool _isCooldown;
    private bool _spinning;

    private Coroutine _spinRoutine;
    private Coroutine _durationRoutine;
    private Coroutine _cooldownRoutine;

    private void Start()
    {
        _targets = new GameObject[_objCount];
        for (int i = 0; i < _objCount; i++)
        {
            _targets[i] = Instantiate(_projectilePrefab, transform);
            // _projectilePrefab 비활성화
            _targets[i].SetActive(false);  
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UseSkill(transform);
    }

    private void UseSkill(Transform attacker)
    {
        // 쿨타임 체크
        if (_isCooldown || _spinning) return;

        // 스킬 사용
        Debug.Log("스킬_2 사용");
        _spinning = true;
        // _projectilePrefab 활성화
        SetTargetsActive(true);  

        // 매 프레임 원 운동 갱신
        _spinRoutine = StartCoroutine(SpinCoroutine());
        // 지속시간 체크 시작
        _durationRoutine = StartCoroutine(SpinDurationCoroutine());
        // 쿨타임 체크 시작
        _isCooldown = true;
        _cooldownRoutine = StartCoroutine(CooldownCoroutine());
    }

    // 원 운동 로직
    private void UpdateProjectiles()
    {
        float interval = 360f / _objCount;
        Vector3 center = transform.position;

        for (int i = 0; i < _objCount; i++)
        {
            float angle = _degree + interval * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Sin(rad) * _radius,
                Mathf.Cos(rad) * _radius,
                0f
            );
            _targets[i].transform.position = center + offset;
        }
    }

    #region Coroutine
    // 원 운동
    private IEnumerator SpinCoroutine()
    {
        //  _projectilePrefab 한 바퀴 회전에 걸리는 시간 : 360f / _objSpeed
        while (_spinning)
        {
            _degree = (_degree + Time.deltaTime * _objSpeed) % 360f;
            UpdateProjectiles();
            yield return null;
        }
    }

    // 지속 시간
    private IEnumerator SpinDurationCoroutine()
    {
        yield return new WaitForSeconds(_spinDuration);
        _spinning = false;
        if (_spinRoutine != null) StopCoroutine(_spinRoutine);
        Debug.Log("스킬 지속 시간 종료");
        // _projectilePrefab 활성화
        SetTargetsActive(false);
    }

    // 쿨타임
    private IEnumerator CooldownCoroutine()
    {
        float remaining = _data.CoolTime;
        while (remaining > 0f)
        {
            Debug.Log($"쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        _isCooldown = false;
        Debug.Log("쿨타임 종료");
    }
    #endregion

    // _projectilePrefab 활성화/비활성화
    private void SetTargetsActive(bool isActive)
    {
        foreach (var t in _targets)
            t.SetActive(isActive);
    }

    private void HandleCollision(Collider2D monster)
    {
        Debug.Log("_projectilePrefab 이벤트");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnEnable()
    {
        Skill_2_Projectile.Skill_2_Event += HandleCollision;
    }

    private void OnDisable()
    {
        Skill_2_Projectile.Skill_2_Event -= HandleCollision;
    }

    private void Damage()
    {
        float damage = _playerController.AttackPoint * (0.75f + 0.0075f * _skillLevel);
        foreach (var monster in _hitMonsters)
        {
            monster.GetComponent<Monster_CYH>().TakeDamage(damage);
            Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
        }
    }
}