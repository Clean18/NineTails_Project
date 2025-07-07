using System.Collections;
using UnityEngine;

public class SkillLogic_1 : SkillLogic, ISkill
{
    [SerializeField] private GameObject _hitBoxPrefab;
    [SerializeField] private GameObject _skillEffectPrefab;

    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private GameObject _skillEffect;
    [SerializeField] private Vector3 _effectSpawnPos;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }
    [field: SerializeField] public float RemainCooldown { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 1 초기화");

        // 각 프리팹 자식으로 생성
        GameObject hitBox = Instantiate(_hitBoxPrefab, transform);
        _hitBox = hitBox.GetComponent<CircleCollider2D>();
        _hitBox.enabled = false;
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = -1;
        _effectSpawnPos = new Vector3(0, 12.22765f, 0);
    }

    public void SkillInit(SaveSkillData playerSkillData)
    {
        SlotIndex = playerSkillData.SlotIndex;
        IsCooldown = playerSkillData.SkillCooldown > 0f;
        if (IsCooldown) PlayerController.Instance.StartCoroutine(CooldownCoroutine(playerSkillData.SkillCooldown));
    }

    public bool UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;
        Debug.Log("스킬 1 UseSkill");

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        EnableHitbox();
        AnimationPlay();
        Debug.Log("스킬 1 사용완료");
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

        EnableHitbox();
        AnimationPlay();
        Debug.Log("스킬 1 사용완료");
        return true;
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

        // 플레이어 움직임 활성화
        PlayerController.Instance.Move();
    }

    public void AnimationPlay()
    {
        if (!_hitBox.enabled) return;
        else PlayerController.Instance.SetTrigger("UseSkill_1");

        // 플레이어 움직임 비활성화
        PlayerController.Instance.Stop();
    }

    public void CreateEffect()
    {
        Debug.Log("CreateEffect");
        GameObject effect = Instantiate(_skillEffectPrefab, transform);
        _skillEffect = effect;
        effect.transform.localPosition = _effectSpawnPos;
    }

    public void DestroyEffect()
    {
        Destroy(_skillEffect);
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
        RemainCooldown = PlayerController.Instance.GetCalculateCooldown(SkillData.CoolTime);
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 {RemainCooldown} 초");
        while (RemainCooldown > 0f)
        {
            yield return new WaitForSeconds(1f);
            RemainCooldown -= 1f;
        }
        RemainCooldown = 0f;
        IsCooldown = false;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 종료");
    }
    private IEnumerator CooldownCoroutine(float reamainCooldown)
    {
        RemainCooldown = reamainCooldown;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 {RemainCooldown} 초");
        while (RemainCooldown > 0f)
        {
            yield return new WaitForSeconds(1f);
            RemainCooldown -= 1f;
        }
        RemainCooldown = 0f;
        IsCooldown = false;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 종료");
    }
}
