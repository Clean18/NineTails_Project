using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    public bool IsCooldown { get; set; }
    public int SkillLevel  { get; set; }
    public int SlotIndex { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public void UseSkill(Transform attacker);
    public void UseSkill(Transform attacker, Transform defender);
    public void SkillInit();
}
