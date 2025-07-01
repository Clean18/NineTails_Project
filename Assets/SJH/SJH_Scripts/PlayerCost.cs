using System;
using Unity.Mathematics;
using UnityEngine;

public struct SavePlayerCost
{
    public long SpiritEnergy;
    public long Warmth;
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
	/// 영기 : 스탯 강화 재화
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
    /// 온정 : 장비 강화 재화
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

    public void InitCost(long spiritEnergy = 0, long warmth = 0)
    {
        SpiritEnergy = spiritEnergy;
        Warmth = warmth;
    }

    public void IncreaseSpiritEnergy(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        Debug.Log($"영기 {amount} 증가");
        SpiritEnergy = math.min(99999999999999999, SpiritEnergy + amount);
    }

    public void IncreaseWarmth(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        Debug.Log($"온기 {amount} 증가");
        Warmth = math.min(99999999999999999, Warmth + amount);
    }

    public void DecreaseSpiritEnergy(long amount)
    {
        Debug.Log($"영기 {amount} 감소");
        SpiritEnergy = math.max(0, SpiritEnergy - amount);
    }

    public void DecreaseWarmth(long amount)
    {
        Debug.Log($"온기 {amount} 감소");
        Warmth = math.max(0, Warmth - amount);
    }

    public SavePlayerCost SavePlayerCost()
    {
        var cost = new SavePlayerCost();
        cost.SpiritEnergy = SpiritEnergy;
        cost.Warmth = Warmth;
        return cost;
    }
}
