using System.Collections;
using UnityEngine;

public class SkillLogic_0_HitBox : SkillLogic, ISkill
{
    [SerializeField] private GameObject _hitBoxPrefab;
    [SerializeField] private PolygonCollider2D _hitBox;
    [SerializeField] private int _slashCount = 0;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    public void SkillInit()
    {
        Debug.Log("기본공격 초기화");

        // 각 프리팹 자식으로 생성
        GameObject hitBox = Instantiate(_hitBoxPrefab, transform);
        _hitBox = hitBox.GetComponent<PolygonCollider2D>();
        _hitBox.enabled = false;
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = 0;
    }

    public bool UseSkill(Transform attacker)
    {
        Debug.Log("기본공격 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        _slashCount = 1;
        // 첫번째 공격은 스크립트에서 실행
        OnAttackStart();
        AnimationPlay();
        return true;
    }

    public bool UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        _slashCount = 1;
        OnAttackStart();
        AnimationPlay();
        return true;
    }

    public void OnAttackStart()
    {
        // OnTrigger 플래그
        _isSkillUsed = true;

        _hitBox.enabled = true;
    }

    // 애니메이션이 끝났을 때 이벤트로 호출
    public void OnAttackEnd()
    {
        _hitBox.enabled = false;

        // 몬스터 TakeDamage 처리
        Damage();

        if (_slashCount == 2) _slashCount = 0;

        // OnTrigger 플래그
        _isSkillUsed = false;
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled) return;
        else PlayerController.Instance.SetTrigger("UseSkill_0");
    }

    // 애니메이션이 끝났을 때 이벤트로 호출
    public void SlashCountEvent()
    {
        // 1타용 리스트 초기화
        _hitMonsters.Clear();

        _slashCount = 2;
    }

    // 각 타마다 _hitMonsters 리스트에 담긴 몬스터에게 한 번씩만 데미지 처리
    protected override void Damage()
    {
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * ((100f + SkillLevel) / 100f));

        if(_slashCount == 1)
        {
            foreach (var monster in _hitMonsters)
            {
                monster?.GetComponent<IDamagable>().TakeDamage(damage);
                Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
            }
        }
        else if(_slashCount == 2)
        {
            foreach (var monster in _hitMonsters)
            {
                monster?.GetComponent<IDamagable>().TakeDamage(damage / 2);
                Debug.Log($"{monster.name}에게 {damage / 2}의 피해를 가했음");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isSkillUsed) return;

        if (!other.CompareTag("Monster")) return;
        if (!_hitMonsters.Contains(other.gameObject))
        {
            _hitMonsters.Add(other.gameObject);
        }
    }

    // 쿨타임 코루틴
    private IEnumerator CooldownCoroutine()
    {
        float remaining = PlayerController.Instance.GetCalculateCooldown(SkillData.CoolTime);
        //Debug.Log($"기본 공격 쿨타임 {remaining} 초");
        while (remaining > 0f)
        {
            //Debug.Log($"기본 공격 쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        IsCooldown = false;
        //Debug.Log("기본 공격 쿨타임 종료");
    }
}