using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

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

	// 플레이어 장비
	public PlayerEquipment Equipment;

    // 플레이어 업적, 미션
    public PlayerQuest Quest;

    /// <summary>
    /// PlayerModel 초기화
    /// </summary>
    public void InitModel(GameData saveData)
	{
		if (saveData == null) return;

		// 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
		Data = new PlayerData();
		Data.InitData(saveData.AttackLevel, saveData.DefenseLevel, saveData.HpLevel, saveData.CurrentHp, saveData.SpeedLevel, saveData.ShieldHp);

		// 재화저장도 추가
		Cost = new PlayerCost();
		Cost.InitCost(saveData.SpiritEnergy, saveData.Warmth);

		// TODO : 플레이어의 저장된 스킬을 등록
		Skill = new PlayerSkill();
		Skill.InitSkill(saveData.PlayerSkillList);

		// 플레이어 장비
		Equipment = new PlayerEquipment();
		Equipment.InitEquipment(saveData.Grade, saveData.Level, saveData.IncreaseDamageLevel);

        Quest = new PlayerQuest();
        Quest.InitQuest(saveData.PlayerAchivementList, saveData.PlayerMissionList);
    }

    public void ApplyDamage(long damage)
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "1-3" || scene == "2-3" || scene == "3-3")
        {
            Debug.Log("[업적 실패] 보스 스테이지에서 피격됨");
        }
        Data.DecreaseHp(damage);
        if (Data.Hp <= 0)
        {
            // TODO : 플레이어 죽음 처리
            //Debug.LogError("플레이어 사망");
            AchievementManager.Instance?.CheckDeathAchievements(); // 플레이어 Death 업적 카운트
        }
    }

    public void ApplyHeal(long amount) => Data.HealHp(amount);
    public void ApplyShield(long amount) => Data.HealShield(amount);

    public bool GetIsDead() => Data.IsDead;
	public long GetPower() => Data.PowerLevel;
	public long GetAttack() => Data.Attack;
	public long GetDefense() => Data.Defense;
	public long GetMaxHp() => Data.MaxHp;
	public long GetHp() => Data.Hp;
	public long GetShieldHp() => Data.ShieldHp;
	public long GetWarmth() => Cost.Warmth;
	public long GetSpiritEnergy() => Cost.SpiritEnergy;
    public long GetSoul() => Cost.Soul;
	public void ClearShield() => Data.ShieldHp = 0;

	public void ConnectEvent(Action playerStatUI)
	{
		Data.OnStatChanged += playerStatUI;
		Cost.OnCostChanged += playerStatUI;
	}

    /// <summary>
    /// 플레이어 모든 데이터 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public GameData GetGameData()
	{
		SavePlayerData data = Data.SavePlayerData();
		SavePlayerCost cost = Cost.SavePlayerCost();
		SaveEquipmentData equip = Equipment.SavePlayerEquipment();
		List<SaveSkillData> skills = Skill.SavePlayerSkill();
        List<SaveAchievementData> achievments = Quest.SaveAchievementData();
        // TODO : 미션 아이디랑, 퍼블릭 테이블 완성되면 주석해제
        List<SaveMissionData> missions = Quest.SaveMissionData();

		GameData gameData = SaveLoadManager.Instance.GameData;

		// Data
		gameData.AttackLevel = data.AttackLevel;
		gameData.DefenseLevel = data.DefenseLevel;
		gameData.SpeedLevel = data.SpeedLevel;
		gameData.HpLevel = data.HpLevel;
		gameData.CurrentHp = data.CurrentHp;
		gameData.ShieldHp = data.ShieldHp;

		// Cost
		gameData.Warmth = cost.Warmth;
		gameData.SpiritEnergy = cost.SpiritEnergy;
        gameData.Soul = cost.Soul;
        gameData.GetFirstWarmth = cost.GetFirstWarmth;
        gameData.GetFirstSpiritEnergy = cost.GetFirstSpiritEnergy;

        // Skill
        gameData.PlayerSkillList = skills;

		// Equipment
		gameData.Grade = equip.Grade;
		gameData.Level = equip.Level;
		gameData.IncreaseDamageLevel = equip.IncreaseDamageLevel;

        // Quest
        gameData.PlayerAchivementList = achievments;
        gameData.PlayerMissionList = missions;

		return gameData;
	}

	#region Cost 관련 함수
	public long GetCost(CostType costType)
	{
		if (costType == CostType.Warmth) return Cost.Warmth;
		else return Cost.SpiritEnergy;
	}

	public void AddCost(CostType costType, long amount)
	{
		if (amount == 0 || Cost == null) return;

		// 플레이어 영기 추가
		if (costType == CostType.Warmth) Cost.IncreaseWarmth(amount);
		else if (costType == CostType.SpiritEnergy) Cost.IncreaseSpiritEnergy(amount);
        else if (costType == CostType.Soul) Cost.IncreaseSoul(amount);
    }

	public void SpendCost(CostType costType, long amount)
	{
		if (amount == 0 || Cost == null || PlayerController.Instance.IsCheat) return;

		// 플레이어 영기 감소
		if (costType == CostType.Warmth) Cost.DecreaseWarmth(amount);
		else if (costType == CostType.SpiritEnergy) Cost.DecreaseSpiritEnergy(amount);
        else if (costType == CostType.Soul) Cost.DecreaseSoul(amount);
	}
	#endregion

	#region PlayerData 관련 함수
	public SavePlayerData GetPlayerData() => Data.SavePlayerData();
	public void TryAttackLevelup()
	{
		// 현재 레벨 체크
		if (Data.AttackLevel == 300)
		{
			Debug.Log("최대 레벨입니다.");
			return;
		}

		// 비용 체크
		long cost = DataManager.Instance.GetStatCost(StatDataType.Attack, Data.AttackLevel);
		if (cost > Cost.Warmth && !PlayerController.Instance.IsCheat)
		{
			Debug.Log($"온기가 부족합니다. {cost} > {Cost.Warmth}");
			return;
		}

		// 비용 감소
		if (!PlayerController.Instance.IsCheat) SpendCost(CostType.Warmth, cost);

		// 레벨업 실행
		Data.AttackLevelup();
	}

	public void TryDefenseLevelup()
	{
		// 현재 레벨 체크
		if (Data.DefenseLevel == 300)
		{
			Debug.Log("최대 레벨입니다.");
			return;
		}

		// 비용 체크
		long cost = DataManager.Instance.GetStatCost(StatDataType.Defense, Data.DefenseLevel);
		if (cost > Cost.Warmth && !PlayerController.Instance.IsCheat)
		{
			Debug.Log($"온기가 부족합니다. {cost} > {Cost.Warmth}");
			return;
		}

		// 비용 감소
		if (!PlayerController.Instance.IsCheat) SpendCost(CostType.Warmth, cost);

		// 레벨업 실행
		Data.DefenseLevelup();
	}

	public void TryHpLevelup()
	{
		// 현재 레벨 체크
		if (Data.HpLevel == 300)
		{
			Debug.Log("최대 레벨입니다.");
			return;
		}

		// 비용 체크
		long cost = DataManager.Instance.GetStatCost(StatDataType.Hp, Data.HpLevel);
		if (cost > Cost.Warmth && !PlayerController.Instance.IsCheat)
		{
			Debug.Log($"온기가 부족합니다. {cost} > {Cost.Warmth}");
			return;
		}

		// 비용 감소
		if (!PlayerController.Instance.IsCheat) SpendCost(CostType.Warmth, cost);

		// 레벨업 실행
		Data.HpLevelup();
	}

	public void TrySpeedLevelup()
	{
		// 현재 레벨 체크
		if (Data.SpeedLevel == 300)
		{
			Debug.Log("최대 레벨입니다.");
			return;
		}

		// 비용 체크
		long cost = DataManager.Instance.GetStatCost(StatDataType.Speed, Data.SpeedLevel);
		if (cost > Cost.Warmth && !PlayerController.Instance.IsCheat)
		{
			Debug.Log($"온기가 부족합니다. {cost} > {Cost.Warmth}");
			return;
		}

		// 비용 감소
		if (!PlayerController.Instance.IsCheat) SpendCost(CostType.Warmth, cost);

		// 레벨업 실행
		Data.SpeedLevelup();
	}

    public string GetPlayerName() => Data.PlayerName;

    #endregion

    #region Equipment 관련 함수
    /// <summary>
    /// PlayerEquipment 클래스 저장용 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public SaveEquipmentData GetEquipmentData() => Equipment.SavePlayerEquipment();
    /// <summary>
    /// 장비 강화를 시도하는 함수
    /// </summary>
    public void TryEnhance() => Equipment.TryEnhance(Cost.Warmth);
    /// <summary>
    /// 장비 승급을 시도하는 함수
    /// </summary>
	public void TryPromote() => Equipment?.TryPromote(Cost.Warmth);
    /// <summary>
    /// 현재 장비 등급을 GradeType으로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public GradeType GetGradeType() => Equipment.GradeType;
    /// <summary>
    /// 현재 장비의 가하는 피해 증가율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetIncreseDamage() => Equipment.IncreaseDamage;
    /// <summary>
    /// level(장비레벨)의 가하는 피해 증가율을 반환하는 함수
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public float GetIncreseDamage(int level) => Equipment.GetIncreseDamage(level);
    /// <summary>
    /// 현재 장비의 스킬 쿨타임 감소율을 반환하는 함수
    /// </summary>
    /// <param name="defaultCooldown"></param>
    /// <returns></returns>
    public float GetCalculateCooldown(float defaultCooldown) => Equipment.GetCalculateCooldown(defaultCooldown);
    /// <summary>
    /// 현재 장비의 공격력 증가율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetEquipmentAttack() => Equipment.Attack;
    #endregion

    #region PlayerSkill 관련 함수
    /// <summary>
    /// PlayerSkill 클래스 저장용 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveSkillData> GetSkillData() => Skill.SavePlayerSkill();
    /// <summary>
    /// skillIndex 스킬 레벨업을 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void TrySkillLevelUp(int skillIndex) => Skill.TrySkillLevelUp(skillIndex);
    /// <summary>
    /// 스킬 단축창을 Dictionary로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public Dictionary<KeyCode, ISkill> GetMappingSkills() => Skill.SkillMapping;
    /// <summary>
    /// 스킬 단축창에 등록되어 있는 스킬을 List로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<ISkill> GetSkillMappingList() => Skill.GetSkillMappingList();
    public List<ISkill> GetHasSkillList() => Skill.HasSkills;
    /// <summary>
    /// skillIndex 번째 스킬을 획득하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void LearnSkill(int skillIndex) => Skill.LearnSkill(skillIndex, Cost.Soul);
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에 추가를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void AddSkillSlot(int skillIndex) => Skill.AddSkillSlot(skillIndex);
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에서 제거를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void RemoveSkillSlot(int skillIndex) => Skill.RemoveSkillSlot(skillIndex);
    #endregion

    #region Quest 관련 함수
    public List<SaveAchievementData> GetAchievData() => Quest.SaveAchievementData();
    public List<SaveMissionData> GetMissionData() => Quest.SaveMissionData();
    #endregion
}
