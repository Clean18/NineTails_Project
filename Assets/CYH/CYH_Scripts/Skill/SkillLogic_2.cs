using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] private Vector3 _centerOffset = Vector3.zero;

    [Header("스킬 지속 시간")]
    [SerializeField] private float _spinDuration = 7f;

    [SerializeField] private int _skillLevel = 0;
    [SerializeField] private List<GameObject> _hitMonsters = new List<GameObject>();

    private GameObject[] _projectile;
    private float _degree;
    private bool _isCooldown;
    private bool _spinning;

    private Coroutine _spinRoutine;
    private Coroutine _durationRoutine;
    private Coroutine _cooldownRoutine;

    private WaitForSeconds _spinDurationWait;
    private WaitForSeconds _cooldownWait;


    private void Start()
    {
        _spinDurationWait = new WaitForSeconds(_spinDuration);
        _cooldownWait = new WaitForSeconds(1f);

        _projectile = new GameObject[_objCount];
        for (int i = 0; i < _objCount; i++)
        {
            _projectile[i] = Instantiate(_projectilePrefab, transform);
            // _projectilePrefab 비활성화
            _projectile[i].SetActive(false);  
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
        SetProjectileActive(true);  

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
        Vector3 center = transform.position + _centerOffset;
        //Vector3 center = transform.position;

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
        yield return _spinDurationWait;
        _spinning = false;
        if (_spinRoutine != null) StopCoroutine(_spinRoutine);
        Debug.Log("스킬 지속 시간 종료");
        // _projectilePrefab 활성화
        SetProjectileActive(false);
    }

    // 쿨타임
    private IEnumerator CooldownCoroutine()
    {
        float remaining = _data.CoolTime;
        while (remaining > 0f)
        {
            //Debug.Log($"쿨타임 남음: {remaining}초");
            yield return _cooldownWait;
            remaining -= 1f;
        }
        _isCooldown = false;
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

    private void Damage()
    {
        float damage = _playerController.AttackPoint * (0.25f + 0.0025f * _skillLevel);
        foreach (var monster in _hitMonsters)
        {
            monster.GetComponent<Monster_CYH>().TakeDamage(damage);
            Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
        }
    }

    private void Damage(GameObject monsters)
    {
        float damage = _playerController.AttackPoint * (0.25f + 0.0025f * _skillLevel);
        monsters.GetComponent<Monster_CYH>().TakeDamage(damage);
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
        //Debug.Log("_projectilePrefab 이벤트");
        var monsterObj = monster.gameObject;
        if (!_hitMonsters.Contains(monsterObj))
        {
            _hitMonsters.Add(monsterObj);
            Debug.Log($"리스트에 {monsterObj.name} 추가");
            Damage(monsterObj);
            StartCoroutine(DamageCooldownCouroutine(monsterObj, 1f));
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