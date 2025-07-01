using System;
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

    // 플레이어 장비
    public PlayerEquipment Equipment;

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
        Skill.InitSkill();

        // 플레이어 장비
        Equipment = new PlayerEquipment();
        Equipment.InitEquipment(saveData.Grade, saveData.Level, saveData.IncreaseDamageLevel);
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

    public void ApplyShield(long amount)
    {
        Data.HealShield(amount);
    }

    public bool GetIsDead() => Data.IsDead;
    public long GetPower() => Data.PowerLevel;
    public long GetAttack() => Data.Attack;
    public long GetDefense() => Data.Defense;
    public long GetMaxHp() => Data.MaxHp;
    public long GetHp() => Data.Hp;
    public long GetWarmth() => Cost.Warmth;
    public long GetSpiritEnergy() => Cost.SpiritEnergy;
    public void ClearShield() => Data.ShieldHp = 0;

    public void ConnectEvent(Action playerStatUI)
    {
        Data.OnStatChanged += playerStatUI;
        Cost.OnCostChanged += playerStatUI;
    }

    public GameData GetGameData()
    {
        SavePlayerData data = Data.SavePlayerData();
        SavePlayerCost cost = Cost.SavePlayerCost();
        SaveEquipmentData equip = Equipment.SavePlayerEquipment();

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

        // TODO : Skill

        // Equipment
        gameData.Grade = equip.Grade;
        gameData.Level = equip.Level;
        gameData.IncreaseDamageLevel = equip.IncreaseDamageLevel;

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
    }

    public void SpendCost(CostType costType, long amount)
    {
        if (amount == 0 || Cost == null || PlayerController.Instance.IsCheat) return;

        // 플레이어 영기 감소
        if (costType == CostType.Warmth) Cost.DecreaseWarmth(amount);
        else if (costType == CostType.SpiritEnergy) Cost.DecreaseSpiritEnergy(amount);
    }
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
            PlayerController.Instance.SpendCost(CostType.Warmth, Equipment.BaseSSRCost);
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
            }
        }
        else
        {
            Debug.Log("승급에 실패했습니다...");
            return;
        }
    }
    #endregion
}
