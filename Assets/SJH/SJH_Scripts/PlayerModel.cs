using System;
using System.Collections.Generic;
using UnityEngine;
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
        // 세이브파일이 없으면 saveData는 null
        if (saveData == null)
        {
            SaveLoadManager.Instance.GameData = InitFirst();
            return;
        }

        // 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
        Data = new PlayerData();
		Data.InitData(saveData.PlayerName, saveData.AttackLevel, saveData.DefenseLevel, saveData.HpLevel, saveData.CurrentHp, saveData.SpeedLevel, saveData.ShieldHp, saveData.SceneIndex);

		// 재화저장도 추가
		Cost = new PlayerCost();
		Cost.InitCost(saveData.SpiritEnergy, saveData.Warmth, saveData.Soul, saveData.GetFirstWarmth, saveData.GetFirstSpiritEnergy);

		// 플레이어의 저장된 스킬을 등록
		Skill = new PlayerSkill();
		Skill.InitSkill(saveData.PlayerSkillList);

		// 플레이어 장비
		Equipment = new PlayerEquipment();
		Equipment.InitEquipment(saveData.Grade, saveData.Level, saveData.IncreaseDamageLevel);

        // 플레이어 업적, 돌파미션
        Quest = new PlayerQuest();
        Quest.InitQuest(saveData.PlayerAchivementList, saveData.PlayerMissionList);
    }
    /// <summary>
    /// 플레이어 처음 시작했을 때 초기화
    /// </summary>
    /// <returns></returns>
    GameData InitFirst()
    {
        GameData data = new GameData();
        Data = new PlayerData();
        Data.InitData();

        Cost = new PlayerCost();
        Cost.InitCost();

        Skill = new PlayerSkill();
        Skill.InitSkill();

        Equipment = new PlayerEquipment();
        Equipment.InitEquipment();

        Quest = new PlayerQuest();
        Quest.InitQuest();

        return data;
    }

    /// <summary>
    /// 플레이어가 피해를 입는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(long damage) => Data.DecreaseHp(damage);
    /// <summary>
    /// 플레이어가 회복하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void ApplyHeal(long amount) => Data.HealHp(amount);
    /// <summary>
    /// 플레이어가 보호막을 얻는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void ApplyShield(long amount) => Data.HealShield(amount);

    /// <summary>
    /// 플레이어 죽음 상태 반환하는 함수
    /// <br/> true = 죽음
    /// <br/> false = 살음
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead() => Data.IsDead;
    /// <summary>
    /// 플레이어의 전투력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetPower() => Data.PowerLevel;
    /// <summary>
    /// 플레이어의 순수 공격력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetAttack() => Data.Attack;
    /// <summary>
    /// 플레이어의 방어력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetDefense() => Data.Defense;
    /// <summary>
    /// 플레이어의 최대 체력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetMaxHp() => Data.MaxHp;
    /// <summary>
    /// 플레이어의 현재 체력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetHp() => Data.Hp;
    /// <summary>
    /// 플레이어의 보호막 수치를 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetShieldHp() => Data.ShieldHp;
    /// <summary>
    /// 플레이어의 온정 보유량을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetWarmth() => Cost.Warmth;
    /// <summary>
    /// 플레이어의 영기 보유량을 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public long GetSpiritEnergy() => Cost.SpiritEnergy;
    /// <summary>
    /// 플레이어의 혼백을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetSoul() => Cost.Soul;
    /// <summary>
    /// 플레이어의 보호막을 없애는 함수
    /// </summary>
	public void ClearShield() => Data.ShieldHp = 0;

    /// <summary>
    /// 플레이어 Data, Cost 변화시 플레이어 스탯 UI를 업데이트하는 이벤트를 연결하는 함수
    /// </summary>
    /// <param name="playerStatUI"></param>
	public void ConnectEvent(Action playerStatUI)
	{
		Data.OnStatChanged += playerStatUI;
		Cost.OnCostChanged += playerStatUI;
	}
    public int GetPlayerSceneIndex() => Data.SceneIndex;
    public void SetPlayerSceneIndex(int index) => Data.SceneIndex = index;

    /// <summary>
    /// 플레이어의 세이브 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public GameData GetGameData()
	{
        if (Data == null || Cost == null || Equipment == null || Skill == null || Quest == null) return default(GameData);

		SavePlayerData data = Data.SavePlayerData();
		SavePlayerCost cost = Cost.SavePlayerCost();
		SaveEquipmentData equip = Equipment.SavePlayerEquipment();
		List<SaveSkillData> skills = Skill.SavePlayerSkill();
        List<SaveAchievementData> achievments = Quest.SaveAchievementData();
        List<SaveMissionData> missions = Quest.SaveMissionData();

		GameData gameData = SaveLoadManager.Instance.GameData;

        // 첫 시작일 때 GameData에 값이 없어서 예외처리
        if (gameData == null) gameData = new GameData();

        // Data
        gameData.PlayerName = data.PlayerName;
        gameData.AttackLevel = data.AttackLevel;
		gameData.DefenseLevel = data.DefenseLevel;
		gameData.SpeedLevel = data.SpeedLevel;
		gameData.HpLevel = data.HpLevel;
		gameData.CurrentHp = data.CurrentHp;
		gameData.ShieldHp = data.ShieldHp;
        gameData.SceneIndex = data.SceneIndex;

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
		if (amount == 0 || Cost == null || GameManager.IsCheat) return;

		// 플레이어 영기 감소
		if (costType == CostType.Warmth) Cost.DecreaseWarmth(amount);
		else if (costType == CostType.SpiritEnergy) Cost.DecreaseSpiritEnergy(amount);
        else if (costType == CostType.Soul) Cost.DecreaseSoul(amount);
	}
    #endregion

    #region PlayerData 관련 함수
    /// <summary>
    /// PlayerData 클래스 저장용 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public SavePlayerData GetPlayerData() => Data.SavePlayerData();
    /// <summary>
    /// 플레이어 공격력 스탯 강화를 시도하는 함수
    /// </summary>
	public void TryAttackLevelup() => Data.TryAttackLevelup(Cost.Warmth);
    /// <summary>
    /// 플레이어 방어력 스탯 강화를 시도하는 함수
    /// </summary>
    public void TryDefenseLevelup() => Data.TryDefenseLevelup(Cost.Warmth);
    /// <summary>
    /// 플레이어 체력 스탯 강화를 시도하는 함수
    /// </summary>
    public void TryHpLevelup() => Data.TryHpLevelup(Cost.Warmth);
    /// <summary>
    /// 플레이어 이동속도 스탯 강화를 시도하는 함수
    /// </summary>
	public void TrySpeedLevelup() => Data.TrySpeedLevelup(Cost.Warmth);
    /// <summary>
    /// 플레이어 이름을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName() => Data.PlayerName;
    /// <summary>
    /// 플레이어의 이름을 지정하는 함수
    /// </summary>
    /// <param name="newName"></param>
    /// <returns></returns>
    public string SetPlayerName(string newName) => Data.PlayerName = newName;

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
    /// <summary>
    /// PlayerQuest 클래스 저장용 업적 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveAchievementData> GetAchievData() => Quest.SaveAchievementData();
    /// <summary>
    /// PlayerQuest 클래스 저장용 돌파미션 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveMissionData> GetMissionData() => Quest.SaveMissionData();
    #endregion
}
