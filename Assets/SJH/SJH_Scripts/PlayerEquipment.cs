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
        }
    }

    [SerializeField] private float _cooldownReduction;
    public float CooldownReduction
    {
        get => _cooldownReduction;
        set
        {
            _cooldownReduction = value;
        }
    }

    [SerializeField] private float _reduceDamage;
    public float ReduceDamage
    {
        get => _reduceDamage;
        set
        {
            _reduceDamage = value;
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

    public SaveEquipmentData SavePlayerEquipment()
    {
        var equip = new SaveEquipmentData();
        equip.Grade = Grade;
        equip.Level = _level;
        equip.IncreaseDamageLevel = IncreaseDamageLevel;
        return equip;
    }
}
