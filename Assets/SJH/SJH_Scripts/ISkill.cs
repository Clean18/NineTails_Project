using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// 스킬 사용 가능 상태
    /// <br/> true : 사용 불가
    /// <br/> false : 사용 가능
    /// </summary>
    public bool IsCooldown { get; set; }
    /// <summary>
    /// 스킬 레벨 0 ~ 100
    /// </summary>
    public int SkillLevel  { get; set; }
    /// <summary>
    /// 단축키 슬롯 번호 (0 : 기본공격, 1 ~ 3 : 단축키, -1 : 미등록)
    /// </summary>
    public int SlotIndex { get; set; }
    /// <summary>
    /// 스킬의 SO 데이터
    /// </summary>
    public ActiveSkillData SkillData { get; set; }
    /// <summary>
    /// 남은 쿨타임
    /// </summary>
    public float RemainCooldown { get; set; }
    public bool UseSkill(Transform attacker);
    public bool UseSkill(Transform attacker, Transform defender);
    public void SkillInit();
    public void SkillInit(SaveSkillData playerSkillData);
}
