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
	[field: SerializeField] public bool IsCooldown { get; private set; } = false;
	[field : SerializeField] public float Range { get; private set; }
	[field : SerializeField] public GameObject SkillPrefab { get; private set; }

	public abstract void UseSkill(Transform attacker);
	public abstract void UseSkill(Transform attacker, Transform defender);
	protected virtual bool TryUseSkill(MonoBehaviour component)
	{
		if (IsCooldown)
		{
			Debug.Log($"{SkillName} 스킬은 현재 쿨타임 중");
			return false;
		}

		Debug.Log($"{SkillName} 스킬 쿨타임 시작");
		StartCooldown(component);
		return true;
	}

	public void StartCooldown(MonoBehaviour component)
	{
		if (IsCooldown) return;
		component.StartCoroutine(CooldownRoutine());
	}

	IEnumerator CooldownRoutine()
	{
		IsCooldown = true;
		yield return new WaitForSeconds(Cooldown);
		IsCooldown = false;
	}
}
