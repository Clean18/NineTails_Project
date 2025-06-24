using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 플레이어의 스탯 데이터 클래스
/// </summary>
[System.Serializable]
public class PlayerData
{
	// baseStat * Mathf.Pow(rate, level - 1);
	public const float AttackRate = 1.12f;
	public const float HpRate = 1.1f;
	public const float LevelupCostRate = 1.15f;

	// TODO : 방어력, 이동속도는 각각의 계산식을 통해 변환될 예정

	// 캐릭터 스텟 레벨 1 ~ 300
	public int Level { get; private set; }

	// 공격력 10 ~ 5,202,220,766,384,660
	public long Attack { get; set; }
	// 방어력 (받피감) 12 ~ 1200
	public int Defense { get; set; }
	// 최대 체력 100 ~ 237,910,090,562,588
	public long MaxHp { get; set; }
	// 체력
	public long Hp { get; set; }
	// 이동 속도
	public int MoveSpeed { get; set; } // 100 ~ 200 / 51레벨까지
	// 체력 재생 (3%)
	public float HpRegen { get; set; }

	// 가하는 피해 증가 (특수 스탯)
	public float IncreaseDamage { get; set; }

	public PlayerData(int level = 1, long attack = 10, int defense = 12, long maxhp = 100, long hp = 100, int moveSpeed = 100, float hpRegen = 0.03f, float increaseDamage = 0)
	{
		Level = level;
		Attack = attack;
		Defense = defense;
		MaxHp = maxhp;
		Hp = hp;
		MoveSpeed = moveSpeed / 50;
		HpRegen = hpRegen;
		IncreaseDamage = increaseDamage;
	}
}
