using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스탯, 재화, 스킬 등의 데이터를 가지는 클래스
/// </summary>
[System.Serializable]
public class PlayerModel
{
	// 캐릭터 스텟
	public PlayerData Data;

	// 재화 > 영기, 온정
	public PlayerCost Cost;

	// 플레이어 스킬
	public PlayerSkill Skill;

    /// <summary>
    /// PlayerModel 초기화
    /// </summary>
    public void InitModel(GameData gameData)
    {
        // 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
        Data = new PlayerData(); 
        Data.InitData(gameData.AttackLevel, gameData.DefenseLevel, gameData.HpLevel, gameData.CurrentHp, gameData.SpeedLevel, gameData.IncreaseDamageLevel, gameData.ShieldHp);

        // 재화저장도 추가
        Cost = new PlayerCost();
        Cost.InitCost(gameData.SpiritEnergy, gameData.Warmth);

        // TODO : 플레이어의 저장된 스킬을 등록
        Skill = new PlayerSkill();
        Skill.InitSkill();
    }

	public void ApplyDamage(long damage)
	{
		Data.DecreaseHp(damage);
        if (Data.Hp <= 0)
        {
            // TODO : 플레이어 죽음 처리
            //Debug.LogError("플레이어 사망");
        }
	}

    public void ApplyHeal(long amount)
    {
        Data.HealHp(amount);
    }

    public GameData GetGameData()
    {
        SavePlayerData playerData = Data.SavePlayerData();

        var gameData = SaveLoadManager.Instance.GameData;
        // Data
        gameData.AttackLevel = Data.AttackLevel;
        gameData.DefenseLevel = Data.DefenseLevel;
        gameData.SpeedLevel = Data.SpeedLevel;
        gameData.HpLevel = Data.HpLevel;
        gameData.CurrentHp = Data.Hp;
        gameData.IncreaseDamageLevel = Data.IncreaseDamageLevel;
        gameData.ShieldHp = Data.ShieldHp;

        // Cost
        gameData.Warmth = Cost.Warmth;
        gameData.SpiritEnergy = Cost.SpiritEnergy;

        return gameData;
    }

    public long GetCost(CostType costType)
    {
        if (costType == CostType.Warmth) return Cost.Warmth;
        else return Cost.SpiritEnergy;
    }

    public void AddCost(CostType costType, long amount)
    {
        if (amount == 0 || Cost == null) return;

        // TODO : 플레이어 온기 추가
        if (costType == CostType.Warmth) Cost.IncreaseWarmth(amount);
        // 플레이어 영기 추가
        else if (costType == CostType.SpiritEnergy) Cost.IncreaseSpiritEnergy(amount);
    }

    public void SpendCost(CostType costType, long amount)
    {
        if (amount == 0 || Cost == null) return;

        // TODO : 플레이어 온기 추가
        if (costType == CostType.Warmth) Cost.DecreaseWarmth(amount);
        // 플레이어 영기 추가
        else if (costType == CostType.SpiritEnergy) Cost.DecreaseSpiritEnergy(amount);
    }
}
