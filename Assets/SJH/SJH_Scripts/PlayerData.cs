using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
	// 캐릭터 스텟
	// 공격력
	public float Attack { get; set; }
	// 방어력 (받피감)
	public float Defense { get; set; }
	// 최대 체력
	public int MaxHp { get; set; }
	// 체력
	public int Hp { get; set; }
	// 이동 속도
	public float MoveSpeed { get; set; }
	// 체력 재생 (3%)
	public float HpRegen { get; set; }

	// 가하는 피해 증가 (특수 스탯)
	public int IncreaseDamage { get; set; }

	public PlayerData(float attack = 1, float defense = 0, int maxhp = 100, int hp = 100, float moveSpeed = 2f, float hpRegen = 0.03f, int increaseDamage = 0)
	{
		Attack = attack;
		Defense = defense;
		MaxHp = maxhp;
		Hp = hp;
		MoveSpeed = moveSpeed;
		HpRegen = hpRegen;
		IncreaseDamage = increaseDamage;
	}
}
