using System;
using UnityEngine;

public struct SaveEquipmentData
{
    public string Grade;
    public int Level;
    public int IncreaseDamageLevel;
}

[System.Serializable]
public class PlayerEquipment
{
    public event Action OnEquipmentChanged;

    // 내부로는 GradeType을 사용 외부는 string
    [SerializeField] private GradeType _grade;
    public string Grade
    {
        get
        {
            switch (_grade)
            {
                case GradeType.Normal: return "N";
                case GradeType.Common: return "R";
                case GradeType.Uncommon: return "SR";
                case GradeType.Rare: return "SSR";
                default: return "N";
            }
        }
        set
        {
            switch (value)
            {
                case "N": _grade = GradeType.Normal; break;
                case "R": _grade = GradeType.Common; break;
                case "SR": _grade = GradeType.Uncommon; break;
                case "SSR": _grade = GradeType.Rare; break;
                default: _grade = GradeType.Normal; break;
            }
            OnEquipmentChanged?.Invoke();
        }
    }
    public GradeType GradeType => _grade;


    [SerializeField] private int _level;
    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            OnEquipmentChanged?.Invoke();
        }
    }

    [SerializeField] private float _attack;
    public float Attack
    {
        get => _attack;
        set
        {
            _attack = value;
            OnEquipmentChanged?.Invoke();
        }
    }

    [SerializeField] private float _cooldownReduction;
    public float CooldownReduction
    {
        get => _cooldownReduction;
        set
        {
            _cooldownReduction = value;
            OnEquipmentChanged?.Invoke();
        }
    }

    [SerializeField] private float _reduceDamage;
    public float ReduceDamage
    {
        get => _reduceDamage;
        set
        {
            _reduceDamage = value;
            OnEquipmentChanged?.Invoke();
        }
    }

    [SerializeField] private long _warmthCost;
    public long WarmthCost;

    /// <summary>
    /// 가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가
    /// </summary>
    [Tooltip("가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가")]
    [field: SerializeField] public float IncreaseDamage;

    /// <summary>
    /// 가하는 피해 증가 레벨
    /// </summary>
    [Tooltip("가하는 피해 증가 레벨")]
    [SerializeField] private int _increaseDamageLevel;
    public int IncreaseDamageLevel
    {
        get => _increaseDamageLevel;
        set
        {
            // 1레벨 0.5% 이후 0.2%씩 증가
            Debug.Log("가하는 피해 증가 계산");
            _increaseDamageLevel = Mathf.Max(0, value);
            if (_increaseDamageLevel == 0) IncreaseDamage = 0f;
            else IncreaseDamage = 0.5f + ((_increaseDamageLevel - 1) * 0.2f);

            OnEquipmentChanged?.Invoke();
        }
    }

    [SerializeField] private long _baseSSRCost;
    public long BaseSSRCost;

    public void InitEquipment(string grade = "N", int level = 1, int increaseDamageLevel = 0)
    {
        Grade = grade;
        _level = level;

        if (grade == "SSR")
        {
            Attack = 0.5f;
            CooldownReduction = 0.3f;
            ReduceDamage = 0.3f;
            WarmthCost = BaseSSRCost;
            IncreaseDamageLevel = level;
        }
        else
        {
            // 등급, 레벨로 데이터매니저에서 데이터받아서 스탯에 지정해주기
            var upgradeInfo = DataManager.Instance.GetEquipmentUpgradeInfo(_grade, _level);
            Attack = upgradeInfo.Attack;
            CooldownReduction = upgradeInfo.CooldownReduction;
            ReduceDamage = upgradeInfo.ReduceDamage;
            WarmthCost = upgradeInfo.WarmthCost;
            IncreaseDamageLevel = 0;
        }
    }

    /// <summary>
    /// Equipment 클래스 저장용 데이터 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public SaveEquipmentData SavePlayerEquipment()
    {
        var equip = new SaveEquipmentData();
        equip.Grade = Grade;
        equip.Level = _level;
        equip.IncreaseDamageLevel = IncreaseDamageLevel;
        return equip;
    }

    /// <summary>
    /// level의 가하는 피해 증가율을 반환하는 함수
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public float GetIncreseDamage(int level) => (0.5f + ((level - 1) * 0.2f));
    /// <summary>
    /// 장비 강화를 시도하는 함수
    /// </summary>
    public void TryEnhance(long warmth)
    {
        var player = PlayerController.Instance;
        // SSR 등급은 무한히 강화가 되는 구조
        if (Grade == "SSR")
        {
            if (warmth < BaseSSRCost && !GameManager.IsCheat)
            {
                Debug.Log("재화가 부족하여 강화를 할 수 없습니다.");
                UIManager.Instance.ShowWarningText("강화에 필요한 재화가 부족합니다.");
                return;
            }
            Level += 1;
            IncreaseDamageLevel += 1;
            if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, BaseSSRCost);
            Debug.Log($"강화 성공! 현재 등급: {Grade}등급, 강화 단계: {Level}강");
            //Debug.Log($"공격력 증가율: {Attack * 100}%" + $"스킬 쿨타임 감소: {CooldownReduction * 100}%" + $"방어력 관통 수치: {ReduceDamage * 100}%" + $"누적 피해 증가: {IncreaseDamage}%");
            Debug.Log($"공격력 증가율: 50%" + $"스킬 쿨타임 감소: 30%" + $"방어력 관통 수치: 30%" + $"누적 피해 증가: {IncreaseDamage}%");

            BaseSSRCost++;          // 강화에 들어가는 재화가 1개씩 증가
            return;
        }
        long nextUpgradeCost = DataManager.Instance.GetEquipmentUpgradeCost(GradeType, Level + 1);

        // 다음등급 체크
        if (nextUpgradeCost == -1)
        {
            Debug.Log($"더 이상 강화할 수 없습니다. {Grade} / {Level}");
            return;
        }

        // 재화 체크
        if (warmth < nextUpgradeCost && !GameManager.IsCheat)
        {
            Debug.Log("재화가 부족합니다");
            UIManager.Instance.ShowWarningText("강화에 필요한 재화가 부족합니다.");
            return;
        }

        // 강화 성공
        // 재화 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, nextUpgradeCost);
        // 다음 강화 스탯 할당
        var nextUpgradeStat = DataManager.Instance.GetEquipmentUpgradeInfo(GradeType, Level + 1);
        InitEquipment(nextUpgradeStat.Grade, nextUpgradeStat.Level, nextUpgradeStat.IncreaseDamageLevel);
        Debug.Log($"강화 성공! 현재 등급: {Grade}등급, 강화 단계: {Level}강");
        Debug.Log($"공격력 증가율: {Attack * 100}%" + $"스킬 쿨타임 감소: {CooldownReduction * 100}%" + $"방어력 관통 수치: {ReduceDamage * 100}%" + $"누적 피해 증가: {IncreaseDamage}%");
        AchievementManager.Instance?.CheckEnhancementAchievements(Level); // 강화 업적 조건 체크
        UIManager.Instance.MainUI?.PlayerStatUI();
    }
    /// <summary>
    /// 장비 승급을 시도하는 함수
    /// </summary>
    public void TryPromote(long warmth)
    {
        // 레벨 50인지 체크
        var player = PlayerController.Instance;
        if (Level < 50)
        {
            Debug.Log($"레벨이 부족합니다. 현재 레벨 : {Level}");
            return;
        }
        if (GradeType == GradeType.Rare)
        {
            Debug.Log("승급할 수 없는 등급입니다.");
            return;
        }

        // 승급 테이블에서 내 장비랑 비교하기
        var nextData = DataManager.Instance.GetEquipmentPromotionInfo(GradeType);
        if (nextData.CurrentGrade != Grade)
        {
            Debug.Log("승급할 수 없는 등급입니다.");
            return;
        }

        // 승급 재화 체크
        if (nextData.WarmthCost > warmth && !GameManager.IsCheat)
        {
            Debug.Log("재화가 부족합니다.");
            UIManager.Instance.ShowWarningText("강화에 필요한 재화가 부족합니다.");
            return;
        }

        // 재화 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, nextData.WarmthCost);

        // 승급 확률 체크
        float rate = UnityEngine.Random.value;
        if (rate <= nextData.SuccessRate)
        {
            if (nextData.UpgradeGrade == "SSR")
            {
                InitEquipment("SSR", 1, 1);
                Debug.Log($"승급 성공! 현재 등급: {Grade}등급, 강화 단계: {Level}강");
                //Debug.Log($"공격력 증가율: {Attack * 100}% 쿨타임 감소: {CooldownReduction * 100}% 방어력 관통 수치: {ReduceDamage * 100}% 누적 피해 증가: {IncreaseDamage}%");
                Debug.Log($"공격력 증가율: 50% 쿨타임 감소: 30% 방어력 관통 수치: 30% 누적 피해 증가: {IncreaseDamage}%");
            }
            else
            {
                InitEquipment(nextData.UpgradeGrade, 1, 0);
                Debug.Log($"승급 성공! 현재 등급: {Grade}등급, 강화 단계: {Level}강");
                Debug.Log($"공격력 증가율: {Attack * 100}% 쿨타임 감소: {CooldownReduction * 100}% 방어력 관통 수치: {ReduceDamage * 100}% 누적 피해 증가: {IncreaseDamage}%");
                AchievementManager.Instance?.CheckPromotionAchievements(nextData.CurrentGrade, nextData.UpgradeGrade, true);
            }
        }
        else
        {
            Debug.Log("승급에 실패했습니다...");
            AchievementManager.Instance?.CheckPromotionAchievements(nextData.CurrentGrade, nextData.UpgradeGrade, false);
            return;
        }
    }
    /// <summary>
    /// 현재 장비의 스킬 쿨타임 감소율을 반환하는 함수
    /// </summary>
    /// <param name="defaultCooldown"></param>
    /// <returns></returns>
    public float GetCalculateCooldown(float defaultCooldown) => defaultCooldown * (1 - CooldownReduction);
}
