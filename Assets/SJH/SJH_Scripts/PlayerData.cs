using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
	// 캐릭터 스텟
	// 공격력
	public float Attack { get; set; } = 1;
	// 방어력 (받피감)
	public float Defense { get; set; } = 0;
	// 체력
	public int Hp { get; set; } = 100;
	// 최대 체력
	public int MaxHp { get; set; } = 100;
	// 이동 속도
	public float MoveSpeed { get; set; } = 5f;
	// 체력 재생 (3%)
	public float HpRegen { get; set; } = 0.03f;

	// 가하는 피해 증가 (특수 스탯)
	public int IncreaseDamage { get; set; } = 0;

	public PlayerData()
	{

	}
}
