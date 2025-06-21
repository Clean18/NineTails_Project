using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "Skill/Fireball")]
public class Fireball : SkillData
{
	public override void UseSkill(SkillData skillData)
	{
		Debug.Log($"{SkillName} 스킬 사용");
	}
}
