using System;
using Unity.Mathematics;
using UnityEngine;

public struct SavePlayerCost
{
    public long SpiritEnergy;           // 저장할 영기
    public long Warmth;                 // 저장할 온정
    public long Soul;                   // 저장할 혼백
    public bool GetFirstWarmth;         // 첫 온정 획득 트리거
    public bool GetFirstSpiritEnergy;   // 첫 영기 획득 트리거
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
            if (!GetFirstSpiritEnergy && value > 0)
            {
                GetFirstSpiritEnergy = true;
                _spiritEnergy = value;
                Debug.Log("첫 영기 획득");
                PlayerController.Instance.SetPlayerSceneIndex(14); // 여기서 세이브도 함
                SceneChangeManager.Instance.LoadNextScene(14);
                return;
            }

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
            if (!GetFirstWarmth && value > 0)
            {
                GetFirstWarmth = true;
                _warmth = value;
                Debug.Log($"첫 온정 획득 : {GetFirstWarmth}");
                // Stage 1-1 Middle 다이얼로그로 이동 5번씬
                PlayerController.Instance.SetPlayerSceneIndex(5); // 여기서 세이브도 함
                SceneChangeManager.Instance.LoadNextScene(5);
                return;
            }
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

    // 첫 온정 획득
    public bool GetFirstWarmth;
    // 첫 영기 획득
    public bool GetFirstSpiritEnergy;

    public void InitCost(long spiritEnergy = 0, long warmth = 0, long soul = 0, bool getFirstWarmth = false, bool getFirstSpiritEnergy = false)
    {
        // 첫 획득을 먼저 지정해줘야 반복안됨
        GetFirstWarmth = getFirstWarmth;
        GetFirstSpiritEnergy = getFirstSpiritEnergy;

        // 재화 할당
        SpiritEnergy = spiritEnergy;
        Warmth = warmth;
        Soul = soul;
    }

    public void IncreaseSpiritEnergy(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        //Debug.Log($"영기 {amount} 증가");
        SpiritEnergy = math.min(99999999999999999, SpiritEnergy + amount);
        AchievementManager.Instance?.CheckCurrencyAchievements();  // 영기 누적 업적 카운트
    }
    public void DecreaseSpiritEnergy(long amount)
    {
        Debug.Log($"영기 {amount} 감소");
        SpiritEnergy = math.max(0, SpiritEnergy - amount);
    }
    public void IncreaseWarmth(long amount)
    {
        // 9경9999조9999억9999만9999 까지 쌓임
        //Debug.Log($"온기 {amount} 증가");
        Warmth = math.min(99999999999999999, Warmth + amount);
        AchievementManager.Instance?.CheckCurrencyAchievements(); // 온정 누적 업적 카운트
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
        cost.GetFirstWarmth = GetFirstWarmth;
        cost.GetFirstSpiritEnergy = GetFirstSpiritEnergy;

        return cost;
    }
}
