using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_5 : SkillLogic, ISkill
{
    [SerializeField] private GameObject _fieldPrefab;

    private GameObject _fieldInstance;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }


    public void SkillInit()
    {
        Debug.Log("스킬 5 초기화");
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = -1;
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        UseSkill(transform);
    //    }
    //}

    public bool UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        Debug.Log("스킬5 사용");

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        AnimationPlay();
        return true;
    }

    public bool UseSkill(Transform attacker, Transform defender)
    {
        Debug.Log("스킬 5 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        Debug.Log("스킬5 사용");

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        AnimationPlay();
        Debug.Log("스킬 5 사용완료");
        return true;
    }

    public void SkillRoutine()
    {
        CreateField(transform.position);
        OnAttackEnd();
    }

    public void OnAttackStart()
    {
        _isSkillUsed = true;

        PlayerController.Instance.Stop();
    }

    public void OnAttackEnd()
    {
        _isSkillUsed = false;

        PlayerController.Instance.Move();
    }

    public void AnimationPlay()
    {
        //_animator.SetTrigger("UseSkill_5");
        PlayerController.Instance.SetTrigger("UseSkill_5");

        // 1초 뒤 플레이어 움직임 활성화
        Invoke("PlayerMove", 1f);
    }

    private void PlayerMove()
    {
        PlayerController.Instance.Move();
    }

    // Field 생성
    private void CreateField(Vector3 position)
    {
        _fieldInstance = Instantiate(_fieldPrefab, position, Quaternion.identity);
    }

    // 0.5초마다 Field에서 넘어온 리스트를 받아서 처리
    private void HandleFieldHits(List<GameObject> monsters)
    {
        _hitMonsters.AddRange(monsters);

        foreach (var monster in _hitMonsters)
        {
            //Debug.Log($"{monster.name}");
            Damage(monster);
        }
        _hitMonsters.Clear();
    }

    protected override void Damage(GameObject monster)
    {
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * (0.12f + 0.0012f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
    }

    private IEnumerator CooldownCoroutine()
    {
        float remaining = PlayerController.Instance.GetCalculateCooldown(SkillData.CoolTime);
        Debug.Log($"5번 스킬 쿨타임 {remaining} 초");
        while (remaining > 0f)
        {
            //Debug.Log($"5번 스킬 쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        IsCooldown = false;
        Debug.Log("5번 스킬 쿨타임 종료");
    }

    private void OnEnable()
    {
        Skill_5_Field.Skill_5_Event += HandleFieldHits;
    }

    private void OnDisable()
    {
        Skill_5_Field.Skill_5_Event -= HandleFieldHits;
    }
}
