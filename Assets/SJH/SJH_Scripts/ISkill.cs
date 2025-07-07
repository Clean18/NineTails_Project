using UnityEngine;

public interface ISkill
{
    public bool IsCooldown { get; set; }
    public int SkillLevel  { get; set; }
    public int SlotIndex { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public bool UseSkill(Transform attacker);
    public bool UseSkill(Transform attacker, Transform defender);
    public void SkillInit();
}
