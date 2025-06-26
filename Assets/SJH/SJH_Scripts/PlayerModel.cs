using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스탯, 재화, 스킬 등의 데이터를 가지는 클래스
/// </summary>
//[System.Serializable]
public class PlayerModel
{
	// 캐릭터 스텟
	public PlayerData Data;

	// 재화 > 영기, 온정
	public PlayerCost Cost;

	// 플레이어 스킬
	public PlayerSkill Skill;

	public PlayerModel()
	{
		// 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
		Data = new PlayerData();
		Cost = new PlayerCost();
		Skill = new PlayerSkill();
	}

	public void ApplyDamage(long damage)
	{
		Data.DecreaseHp(damage);
        if (Data.Hp <= 0)
        {
            Debug.Log("플레이어 사망");
        }
	}
}
