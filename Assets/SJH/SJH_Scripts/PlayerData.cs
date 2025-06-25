using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 플레이어의 스탯 데이터 클래스
/// </summary>
[System.Serializable]
public class PlayerData
{
	// baseStat * Mathf.Pow(rate, level - 1);
	public const double AttackRate = 1.12f;
    public const int AttackBase = 10;
	public const float HpRate = 1.1f;
	public const float DefenseRate = 1.08f;
	public const float LevelupCostRate = 1.15f;

	// TODO : 방어력, 이동속도는 각각의 계산식을 통해 변환될 예정

    /// <summary>
    /// 플레이어 전투력
    /// </summary>
    public long PowerLevel { get; private set; }

    /// <summary>
    /// 공격력 10 ~ 5,202,220,766,384,660
    /// </summary>
    public long Attack { get; private set; }
    /// <summary>
    /// 공격력 레벨 1 ~ 300
    /// </summary>
    public int AttackLevel { get; private set; }

    /// <summary>
    /// 방어력 (받피감) 12 ~ 1200
    /// </summary>
    public int Defense { get; private set; }
    /// <summary>
    /// 방어력 레벨 1 ~ 300
    /// </summary>
    public int DefenseLevel { get; private set; }

    /// <summary>
    /// 최대 체력 100 ~ 237,910,090,562,588
    /// </summary>
    public long MaxHp { get; private set; }
    /// <summary>
    /// 체력 레벨 1 ~ 300
    /// </summary>
    public long HpLevel { get; private set; }

	/// <summary>
    /// 현재 체력
    /// </summary>
	public long Hp { get; private set; }

    /// <summary>
    /// 이동 속도 100 ~ 200, 이동속도 / 50 = 유니티 이속
    /// </summary>
    public int Speed { get; private set; }
    /// <summary>
    /// 이동 속도 레벨 51레벨까지
    /// </summary>
    public int SpeedLevel { get; private set; }

    /// <summary>
    /// 가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가
    /// </summary>
    public float IncreaseDamage { get; private set; }
    /// <summary>
    /// 가하는 피해 증가 레벨
    /// </summary>
    public int IncreaseDamageLevel { get; private set; }

	public PlayerData(int attackLevel = 115, int defenseLevel = 1, int hpLevel = 1, int speedLevel = 1, int increaseDamageLevel = 0)
	{
		AttackLevel = attackLevel;
        DefenseLevel = defenseLevel;
        HpLevel = hpLevel;
        Speed = speedLevel; // / 50;
		IncreaseDamageLevel = increaseDamageLevel;

        SetAttack(AttackLevel);
        TestAttack();

    }

    // TODO : 유저가 스탯을 올리는 방식
    public void SetAttack(int attackLevel)
    {
        double value = AttackBase * Math.Pow(AttackRate, attackLevel - 1);
        long result = (long)Math.Round(value);
        Debug.Log($"{AttackLevel} 레벨 공격력 : {result}");

        //baseStat * Mathf.Pow(rate, level - 1);
    }

    void TestAttack()
    {
        for (int i = 1; i <= 300; i++)
        {
            double value = Math.Pow(AttackRate, i - 1) * AttackBase;
            long attack = (long)Math.Round(value);
            Debug.Log($"{i} 레벨 공격력 : {attack}");
        }
    }

    public void DecreaseHp(long damage)
    {
        Hp -= damage;
        if (Hp <= 0) Hp = 0;
    }
}
