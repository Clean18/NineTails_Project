using System;
using Unity.Mathematics;
using UnityEngine;

public struct SavePlayerCost
{
    public long SpiritEnergy;
    public long Warmth;
    public long Soul;
}

/// <summary>
/// 플레이어의 스탯 및 장비 성장 재화
/// </summary>
[System.Serializable]
public class PlayerCost
{
    /// <summary>
    /// 재화 획득시 UI 업데이트 이벤트
    /// </summary>
    public event Action OnCostChanged;

    [SerializeField] private long _spiritEnergy;
	/// <summary>
	/// 영기 : 장비, 스킬 강화 재화
    /// <br/> 요괴 처치(10%), 업적, 미션 보상에서 획득
	/// </summary>
	public long SpiritEnergy
    {
        get => _spiritEnergy;
        set
        {
            _spiritEnergy = value;
            OnCostChanged?.Invoke();
        }
    }

    [SerializeField] private long _warmth;
    /// <summary>
    /// 온정 : 스탯 강화 재화
    /// <br/> 요괴 처치(확정), 업적, 미션 보상에서 획득
    /// </summary>
    public long Warmth
    {
        get => _warmth;
        set
        {
            _warmth = value;
            OnCostChanged?.Invoke();
        }
    }

    [SerializeField] private long _soul;
    public long Soul
    {
        get => _soul;
        set
        {
            _soul = value;
            OnCostChanged?.Invoke();
        }
    }

    public void InitCost(long spiritEnergy = 0, long warmth = 0, long soul = 0)
    {
        SpiritEnergy = spiritEnergy;
        Warmth = warmth;
        Soul = soul;
    }

    public void IncreaseSpiritEnergy(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        Debug.Log($"영기 {amount} 증가");
        SpiritEnergy = math.min(99999999999999999, SpiritEnergy + amount);
    }
    public void DecreaseSpiritEnergy(long amount)
    {
        Debug.Log($"영기 {amount} 감소");
        SpiritEnergy = math.max(0, SpiritEnergy - amount);
    }
    public void IncreaseWarmth(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        Debug.Log($"온기 {amount} 증가");
        Warmth = math.min(99999999999999999, Warmth + amount);
    }
    public void DecreaseWarmth(long amount)
    {
        Debug.Log($"온기 {amount} 감소");
        Warmth = math.max(0, Warmth - amount);
    }
    public void IncreaseSoul(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        Debug.Log($"혼백 {amount} 증가");
        Soul = math.min(99999999999999999, Soul + amount);
    }
    public void DecreaseSoul(long amount)
    {
        Debug.Log($"혼백 {amount} 감소");
        Soul = math.max(0, Soul - amount);
    }

    public SavePlayerCost SavePlayerCost()
    {
        var cost = new SavePlayerCost();
        cost.SpiritEnergy = SpiritEnergy;
        cost.Warmth = Warmth;
        cost.Soul = Soul;
        return cost;
    }
}
