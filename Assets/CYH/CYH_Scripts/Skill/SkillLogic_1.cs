using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_1 : SkillLogic, ISkill
{
    [SerializeField] private GameObject _hitBoxPrefab;
    [SerializeField] private CircleCollider2D _hitBox;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 1 초기화");

        // 각 프리팹 자식으로 생성
        GameObject hitBox = Instantiate(_hitBoxPrefab, transform);
        _hitBox = hitBox.GetComponent<CircleCollider2D>();
        _hitBox.enabled = false;
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = 1;
    }

    public void UseSkill(Transform attacker)
    {
        Debug.Log("스킬 1 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        EnableHitbox();
        AnimationPlay();
        Debug.Log("스킬 1 사용완료");
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        EnableHitbox();
        AnimationPlay();
    }

    public void EnableHitbox()
    {
        // OnTrigger 플래그
        _isSkillUsed = true;

        _hitBox.enabled = true;
    }

    // 이벤트 함수
    public void DisableHitbox()
    {
        _hitBox.enabled = false;

        // 몬스터 TakeDamage 처리
        Damage();

        // OnTrigger 플래그
        _isSkillUsed = false;
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled) return;
        else PlayerController.Instance.SetTrigger("UseSkill_1");
    }

    protected override void Damage()
    {
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * ((0.75f + 0.0075f * SkillLevel)));

        foreach (var monster in _hitMonsters)
        {
            monster?.GetComponent<IDamagable>().TakeDamage(damage);
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
    }

    private IEnumerator CooldownCoroutine()
    {
        float remaining = SkillData.CoolTime;
        while (remaining > 0f)
        {
            Debug.Log($"쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        IsCooldown = false;
        Debug.Log("쿨타임 종료");
    }
}
