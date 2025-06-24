using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스탯 데이터 클래스
/// </summary>
public class PlayerData
{
	// TODO : 방어력, 이동속도는 각각의 계산식을 통해 변환될 예정

	// 캐릭터 스텟
	// 공격력
	public float Attack { get; set; }
	// 방어력 (받피감)
	public int Defense { get; set; }
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

	public PlayerData(float attack = 1, int defense = 0, int maxhp = 100, int hp = 100, float moveSpeed = 2f, float hpRegen = 0.03f, int increaseDamage = 0)
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
