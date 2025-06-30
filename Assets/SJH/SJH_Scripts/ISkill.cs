using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    public PlayerController PlayerController { get; set; }
    public bool IsCooldown { get; set; }
    public int SkillLevel  { get; set; }
    public ActiveSkillData SkillData { get; set; }
    public void UseSkill(Transform attacker);
    public void UseSkill(Transform attacker, Transform defender);
}
