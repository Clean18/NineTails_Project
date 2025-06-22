using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public abstract class SkillData : ScriptableObject
{
	[field : SerializeField] public string SkillName { get; private set; }
	[field : SerializeField] public string Description { get; private set; }
	[field : SerializeField] public float Damage { get; private set; }
	[field : SerializeField] public float Cooldown { get; private set; }
	[field : SerializeField] public GameObject SkillPrefab { get; private set; }

	public abstract void UseSkill(Transform attacker);
    // TODO : 스킬 정보 추가
}
