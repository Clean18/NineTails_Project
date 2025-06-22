using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "Skill/Fireball")]
public class Fireball : SkillData
{
	public override void UseSkill(Transform attacker)
	{
		Vector3 attackDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		attackDir.z = 0;

		Vector3 spawnPos = attacker.position;
		Vector3 dir = (attackDir - spawnPos).normalized;



		var go = Instantiate(SkillPrefab, spawnPos, Quaternion.identity);
		go.transform.up = dir;
		go.GetComponent<Rigidbody2D>().velocity = dir * 10;

		Debug.Log($"{SkillName} 스킬 사용");
	}
}
