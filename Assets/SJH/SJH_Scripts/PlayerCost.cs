using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스탯 및 장비 성장 재화
/// </summary>
public class PlayerCost
{
	/// <summary>
	/// 영기 : 스탯 강화 재화
	/// </summary>
	public int SpiritEnergy { get; set; }

	/// <summary>
	/// 온정 : 장비 강화 재화
	/// </summary>
	public int Warmth { get; set; }

	public PlayerCost(int spiritEnergy = 0, int warmth = 0)
    {
		SpiritEnergy = spiritEnergy;
		Warmth = warmth;
    }
}
