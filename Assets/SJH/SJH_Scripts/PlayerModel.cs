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

	public void ApplyHeal(long amount)
	{
		Data.HealHp(amount);
	}

	public void ApplyShield(long amount)
	{
		Data.HealShield(amount);
	}

	public bool GetIsDead() => Data.IsDead;
	public long GetPower() => Data.PowerLevel;
	public long GetAttack() => Data.Attack + (long)(Data.Attack * Equipment.Attack);
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
    public SaveEquipmentData GetEquipmentData() => Equipment.SavePlayerEquipment();
	public void TryEnhance()
	{
		var player = PlayerController.Instance;
		// SSR 등급은 무한히 강화가 되는 구조
		if (Equipment.Grade == "SSR")
		{
			if (Cost.Warmth < Equipment.BaseSSRCost && !PlayerController.Instance.IsCheat)
			{
				Debug.Log("재화가 부족하여 강화를 할 수 없습니다.");
				return;
			}
			Equipment.Level += 1;
			Equipment.IncreaseDamageLevel += 1;
			if (!PlayerController.Instance.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, Equipment.BaseSSRCost);
			Debug.Log($"강화 성공! 현재 등급: {Equipment.Grade}등급, 강화 단계: {Equipment.Level}강");
			Debug.Log($"공격력 증가율: {Equipment.Attack * 100}%" + $"스킬 쿨타임 감소: {Equipment.CooldownReduction * 100}%" + $"방어력 관통 수치: {Equipment.ReduceDamage * 100}%" + $"누적 피해 증가: {Equipment.IncreaseDamage}%");

			Equipment.BaseSSRCost++;          // 강화에 들어가는 재화가 1개씩 증가
			return;
		}
		long nextUpgradeCost = DataManager.Instance.GetEquipmentUpgradeCost(Equipment.GradeType, Equipment.Level + 1);

		// 다음등급 체크
		if (nextUpgradeCost == -1)
		{
			Debug.Log($"더 이상 강화할 수 없습니다. {Equipment.Grade} / {Equipment.Level}");
			return;
		}

		// 재화 체크
		if (Cost.Warmth < nextUpgradeCost && !PlayerController.Instance.IsCheat)
		{
			Debug.Log("재화가 부족합니다");
			return;
		}

		// 강화 성공
		// 재화 감소
		if (!PlayerController.Instance.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, nextUpgradeCost);
		// 다음 강화 스탯 할당
		var nextUpgradeStat = DataManager.Instance.GetEquipmentUpgradeInfo(Equipment.GradeType, Equipment.Level + 1);
		Equipment.InitEquipment(nextUpgradeStat.Grade, nextUpgradeStat.Level, nextUpgradeStat.IncreaseDamageLevel);
		Debug.Log($"강화 성공! 현재 등급: {Equipment.Grade}등급, 강화 단계: {Equipment.Level}강");
		Debug.Log($"공격력 증가율: {Equipment.Attack * 100}%" + $"스킬 쿨타임 감소: {Equipment.CooldownReduction * 100}%" + $"방어력 관통 수치: {Equipment.ReduceDamage * 100}%" + $"누적 피해 증가: {Equipment.IncreaseDamage}%");
        AchievementManager.Instance?.CheckEnhancementAchievements(Equipment.Level); // 강화 업적 조건 체크
        UIManager.Instance.MainUI?.PlayerStatUI();
	}
	public void TryPromote()
	{
		// 레벨 50인지 체크
		var player = PlayerController.Instance;
		if (Equipment.Level < 50)
		{
			Debug.Log($"레벨이 부족합니다. 현재 레벨 : {Equipment.Level}");
			return;
		}
		if (Equipment.GradeType == GradeType.Rare)
		{
			Debug.Log("승급할 수 없는 등급입니다.");
			return;
		}

		// 승급 테이블에서 내 장비랑 비교하기
		var nextData = DataManager.Instance.GetEquipmentPromotionInfo(Equipment.GradeType);
		if (nextData.CurrentGrade != Equipment.Grade)
		{
			Debug.Log("승급할 수 없는 등급입니다.");
			return;
		}

		// 승급 재화 체크
		if (nextData.WarmthCost > Cost.Warmth && !PlayerController.Instance.IsCheat)
		{
			Debug.Log("재화가 부족합니다.");
			return;
		}

		// 재화 감소
		if (!PlayerController.Instance.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, nextData.WarmthCost);

		// 승급 확률 체크
		float rate = UnityEngine.Random.value;
		if (rate <= nextData.SuccessRate)
		{
			if (nextData.UpgradeGrade == "SSR")
			{
				Equipment.InitEquipment("SSR", 1, 1);
				Debug.Log($"승급 성공! 현재 등급: {Equipment.Grade}등급, 강화 단계: {Equipment.Level}강");
				Debug.Log($"공격력 증가율: {Equipment.Attack * 100}% 쿨타임 감소: {Equipment.CooldownReduction * 100}% 방어력 관통 수치: {Equipment.ReduceDamage * 100}% 누적 피해 증가: {Equipment.IncreaseDamage}%");

			}
			else
			{
				Equipment.InitEquipment(nextData.UpgradeGrade, 1, 0);
				Debug.Log($"승급 성공! 현재 등급: {Equipment.Grade}등급, 강화 단계: {Equipment.Level}강");
				Debug.Log($"공격력 증가율: {Equipment.Attack * 100}% 쿨타임 감소: {Equipment.CooldownReduction * 100}% 방어력 관통 수치: {Equipment.ReduceDamage * 100}% 누적 피해 증가: {Equipment.IncreaseDamage}%");
                AchievementManager.Instance?.CheckPromotionAchievements(nextData.CurrentGrade, nextData.UpgradeGrade,true);
            }
		}
		else
		{
			Debug.Log("승급에 실패했습니다...");
            AchievementManager.Instance?.CheckPromotionAchievements(nextData.CurrentGrade, nextData.UpgradeGrade,false);
            return;
		}
	}
    public GradeType GetGradeType() => Equipment.GradeType;
    public float GetIncreseDamage() => Equipment.GetIncreseDamage();
    public float GetIncreseDamage(int level) => Equipment.GetIncreseDamage(level);
    #endregion

    #region PlayerSkill 관련 함수
    public List<SaveSkillData> GetSkillData() => Skill.SavePlayerSkill();
    public void TrySkillLevelUp(int skillIndex)
    {
        var player = PlayerController.Instance;
        var skill = player.SkillController.SkillList[skillIndex];
        if (skill == null)
        {
            Debug.Log("스킬이 없습니다.");
            return;
        }
        // 레벨 체크
        if (skill.SkillLevel >= 100)
        {
            Debug.Log("스킬 레벨을 더 이상 올릴 수 없습니다.");
            return;
        }
        // 재화 체크
        // TODO : 스킬이 노말인지 궁극기인지 구분해야함
        bool isUlt = skill.SkillData.SkillIndex == 6;

        if (isUlt) // 궁극기 강화
        {
            var ultCost = DataManager.Instance.GetUltSkillCost(skill.SkillLevel);
            if (ultCost == int.MaxValue)
            {
                Debug.Log("스킬을 강화할 수 없습니다.");
                return;
            }
            // 노말체크
            if (player.GetSpiritEnergy() < ultCost)
            {
                Debug.Log("영기가 부족합니다.");
                return;
            }
            // 재화 감소
            player.SpendCost(CostType.SpiritEnergy, ultCost);
        }
        else // 노말 강화
        {
            var normalCost = DataManager.Instance.GetNormalSkillCost(skill.SkillLevel);
            if (normalCost == int.MaxValue)
            {
                Debug.Log("스킬을 강화할 수 없습니다.");
                return;
            }
            if (player.GetSpiritEnergy() < normalCost)
            {
                Debug.Log("영기가 부족합니다.");
                return;
            }
            player.SpendCost(CostType.SpiritEnergy, normalCost);
        }
        /*
         * 플레이어의 스킬 보유 여부를 체크안한다면
         * UI창에서는 가지고 있는 스킬만 활성화하는 방식
         */
        skill.SkillLevel += 1;
        Debug.Log($"스킬 레벨업! : {skill.SkillData.SkillName} Lv. {skill.SkillLevel}");
    }
    public Dictionary<KeyCode, ISkill> GetMappingSkills() => Skill.SkillMapping;
    public List<ISkill> GetSkillMappingList()
    {
        var list = new List<ISkill>();
        foreach (var skill in Skill.SkillMapping.Values)
        {
            if (skill != null) list.Add(skill);
        }
        return list;
    }
    public List<ISkill> GetHasSkillList() => Skill.HasSkills;
    public void AddSkill(int skillIndex)
    {
        // 중복 체크
        var skill = PlayerController.Instance.SkillController.SkillList[skillIndex];
        if (skill == null)
        {
            Debug.Log("배울 수 없는 스킬입니다.");
            return;
        }
        var mapping = GetSkillMappingList();
        var has = Skill.HasSkills;
        
        foreach (var mappingSkill in mapping)
        {
            if (mappingSkill.SkillData.SkillIndex == skill.SkillData.SkillIndex)
            {
                Debug.Log("이미 배운 스킬입니다.");
                return;
            }
        }
        foreach (var mappingSkill in has)
        {
            if (mappingSkill.SkillData.SkillIndex == skill.SkillData.SkillIndex)
            {
                Debug.Log("이미 배운 스킬입니다.");
                return;
            }
        }

        // 플레이어 재화 체크
        if (Cost.Soul < 1 && !PlayerController.Instance.IsCheat)
        {
            Debug.Log("혼백이 부족합니다.");
            return;
        }

        // 재화 감소
        if (!PlayerController.Instance.IsCheat) Cost.DecreaseSoul(1);

        // 스킬 추가
        Skill.AddSkill(skillIndex);
        // 버튼 업데이트
        SkillButton.Instance.UpdateButtonImage();
    }

    public void AddSkillSlot(int skillIndex)
    {
        var mapping = GetMappingSkills();
        var has = GetHasSkillList();
        ISkill skill = null;

        // 추가할 스킬이 가지고 있는 스킬인지 체크
        bool isHas = false;
        for (int i = 0; i < has.Count; i++)
        {
            if (has[i].SkillData.SkillIndex == skillIndex)
            {
                skill = has[i];
                isHas = true;
                break;
            }
        }

        if (skill == null || isHas == false)
        {
            Debug.Log("가지고 있지 않은 스킬입니다.");
            return;
        }

        if (mapping.ContainsValue(skill))
        {
            Debug.Log("이미 등록된 스킬입니다.");
            return;
        }

        KeyCode[] keyList = new KeyCode[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2, 
            KeyCode.Alpha3
        };

        for (int i = 0; i < keyList.Length; i++)
        {
            KeyCode key = keyList[i];
            if (mapping.TryGetValue(key, out ISkill value) && value == null)
            {
                // 단축키 등록
                skill.SlotIndex = i + 1;
                mapping[key] = skill;
                // hasSkill에서 삭제
                has.Remove(skill);
                Debug.Log($"{skill.SkillData.SkillName} 스킬 {skill.SlotIndex}번 슬롯에 등록");
                return;
            }
        }

        Debug.Log("등록 가능한 슬롯이 없습니다.");
    }

    public void RemoveSkillSlot(int skillIndex)
    {
        var mapping = GetMappingSkills();
        var has = Skill.HasSkills;

        if (skillIndex < 1 || skillIndex > 6) return;

        ISkill skill = null;

        // 슬롯 삭제
        foreach (var pair in mapping)
        {
            // 단축키에서 스킬인덱스와 같은 스킬 찾기
            if (pair.Value != null && pair.Value.SkillData.SkillIndex == skillIndex)
            {
                skill = pair.Value;
                mapping[pair.Key] = null;
                break;
            }
        }

        if (skill == null)
        {
            Debug.Log($"SkillIndex : {skillIndex} 스킬은 등록된 스킬이 아닙니다.");
        }

        // 삭제되면 has리스트에 추가
        if (!has.Contains(skill))
        {
            var debug = skill.SlotIndex;
            Debug.Log($"{skill.SkillData.SkillName} 스킬 {debug}번 슬롯에서 제거");
            skill.SlotIndex = -1;
            has.Add(skill);
        }
    }
    #endregion

    #region Quest 관련 함수
    public List<SaveAchievementData> GetAchievData() => Quest.SaveAchievementData();
    public List<SaveMissionData> GetMissionData() => Quest.SaveMissionData();
    #endregion
}
