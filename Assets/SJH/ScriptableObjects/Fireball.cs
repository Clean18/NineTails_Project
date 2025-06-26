using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "Skill/Fireball")]
public class Fireball : SkillData
{
	public override void UseSkill(Transform attacker)
	{
		if (!TryUseSkill(GameManager.Instance)) return;

		Vector3 attackDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		attackDir.z = 0;

		Vector3 spawnPos = attacker.position;
		Vector3 dir = (attackDir - spawnPos).normalized;

		var go = Instantiate(SkillPrefab, spawnPos, Quaternion.identity);
		go.transform.up = dir;
		go.GetComponent<Rigidbody2D>().velocity = dir * 10;
		go.GetComponent<Projectile>().TotalDamage = Damage;

		Debug.Log($"{SkillName} 스킬 사용");
	}

    public override void UseSkill(Transform attacker, Transform defender)
	{
		if (!TryUseSkill(GameManager.Instance)) return;

		Vector3 attackDir = defender.position;
		attackDir.z = 0;

		Vector3 spawnPos = attacker.position;
		Vector3 dir = (attackDir - spawnPos).normalized;

        var player = GameManager.Instance.PlayerController;

		var go = Instantiate(SkillPrefab, spawnPos, Quaternion.identity);
		go.transform.up = dir;
		go.GetComponent<Rigidbody2D>().velocity = dir * 10;
		var projectile = go.GetComponent<Projectile>().TotalDamage = Damage * (int)player.PlayerModel.Data.Attack;

		Debug.Log($"{SkillName} 스킬 사용 대미지 : {projectile}");
	}
}
