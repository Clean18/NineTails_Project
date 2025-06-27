using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_1 : MonoBehaviour, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private CircleCollider2D _hitBox;

    [SerializeField] private int _skillLevel = 0;

    private Animator _animator;
    private bool _isCooldown = false;

    [SerializeField] private List<GameObject> _hitMonsters = new List<GameObject>();

    public PlayerController PlayerController { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public bool IsCooldown { get; set; }

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
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
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

        EnableHitbox();
        AnimationPlay();
        //Debug.Log("스킬사용");
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

        EnableHitbox();
        AnimationPlay();
        //Debug.Log("스킬사용");
    }

    public void EnableHitbox()
    {
        _hitBox.enabled = true;
        //Debug.Log("콜라이더 켜짐");
    }

    public void DisableHitbox()
    {
        _hitBox.enabled = false;
        //Debug.Log("콜라이더 끔");

        // 몬스터 TakeDamage 처리
        Damage();
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled)
            return;
        else
        {
            _animator.SetTrigger("UseSkill_1");
        }
    }


    private void Damage()
    {
        //float damage = _playerController.AttackPoint * (0.75f + 0.0075f * _skillLevel);
        float damage = PlayerController.PlayerModel.Data.Attack * (0.75f + 0.0075f * _skillLevel);

        foreach (var monster in _hitMonsters)
        {
            //monster.GetComponent<Monster_CYH>().TakeDamage(damage);
            monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
            Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Monster")) return;
        if (!_hitMonsters.Contains(other.gameObject))
        {
            _hitMonsters.Add(other.gameObject);
        }
        Debug.Log($"Skill_1 : 몬스터 맞음");
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
}
