using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Debug = UnityEngine.Debug;

/// <summary>
/// 플레이어의 스탯 데이터 클래스
/// </summary>
//[System.Serializable]
public class PlayerData
{
    /// <summary>
    /// 플레이어 전투력
    /// </summary>
    public long PowerLevel { get; private set; }

    /// <summary>
    /// 공격력 10 ~ 5,202,220,766,384,660
    /// </summary>
    public long Attack { get; private set; }
    private int _attackLevel;
    /// <summary>
    /// 공격력 레벨 1 ~ 300
    /// </summary>
    public int AttackLevel
    {
        get => _attackLevel;
        private set
        {
            _attackLevel = Mathf.Clamp(value, 1, 300);
            Attack = GameManager.Instance.StatDic[StatType.Attack][_attackLevel];
        }
    }

    /// <summary>
    /// 방어력 (받피감) 12 ~ 1200
    /// </summary>
    public long Defense { get; private set; }
    private int _defenseLevel;
    /// <summary>
    /// 방어력 레벨 1 ~ 300
    /// </summary>
    public int DefenseLevel
    {
        get => _defenseLevel;
        private set
        {
            _defenseLevel = Mathf.Clamp(value, 1, 300);
            Defense = GameManager.Instance.StatDic[StatType.Defense][_defenseLevel];
        }
    }

    /// <summary>
    /// 최대 체력 100 ~ 237,910,090,562,588
    /// </summary>
    public long MaxHp { get; private set; }
    private int _hpLevel;
    /// <summary>
    /// 체력 레벨 1 ~ 300
    /// </summary>
    public int HpLevel {
        get => _hpLevel;
        private set
        {
            _hpLevel = Mathf.Clamp(value, 1, 300);
            MaxHp = GameManager.Instance.StatDic[StatType.Hp][_hpLevel];
        }
    }

	/// <summary>
    /// 현재 체력
    /// </summary>
	public long Hp { get; private set; }

    /// <summary>
    /// 이동 속도 100 ~ 200, 이동속도 / 50 = 유니티 이속
    /// </summary>
    public float Speed { get; private set; }
    public int _speedLevel;
    /// <summary>
    /// 이동 속도 레벨 51레벨까지
    /// </summary>
    public int SpeedLevel {
        get => _speedLevel;
        private set
        {
            _speedLevel = Mathf.Clamp(value, 1, 50);
            Speed = GameManager.Instance.StatDic[StatType.Speed][_speedLevel] / _speedRatio;
        }
    }
    private const int _speedRatio = 50;

    /// <summary>
    /// 가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가
    /// </summary>
    public float IncreaseDamage { get; private set; }
    /// <summary>
    /// 가하는 피해 증가 레벨
    /// </summary>
    public int IncreaseDamageLevel { get; private set; }

	public PlayerData(int attackLevel = 1, int defenseLevel = 1, int hpLevel = 1, int speedLevel = 1, int increaseDamageLevel = 0)
	{
        // TODO : HP는 따로 저장해야할듯

        // 프로퍼티에서 레벨만으로 각 스탯 계산
		AttackLevel = attackLevel;
        DefenseLevel = defenseLevel;
        HpLevel = hpLevel;
        SpeedLevel = speedLevel;
		IncreaseDamageLevel = increaseDamageLevel;
    }

    public void DecreaseHp(long damage)
    {
        Hp -= damage;
        if (Hp <= 0) Hp = 0;
    }
}
