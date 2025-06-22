using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
	// 캐릭터 스텟
	public PlayerData Data;

	// 재화 > 영기, 온정
	public PlayerCost Cost;

	public PlayerModel()
	{
		// 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
		Data = new PlayerData();
		Cost = new PlayerCost();
	}
}
