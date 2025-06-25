using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	// 플레이어가 오토로 돌아갈때는 몬스터의 정보를 알아야함 > 몬스터를 추격하고 공격하기 위해
	// 즉, 싱글톤이든 static이든 오브젝트풀이랑 몬스터들의 정보를 플레이어에서 접근할 수 있던가 해야함

	public PlayerController PlayerController;
	public Spawner Spawner;

	public Dictionary<string, SkillData> SkillDic;

	void Start()
	{
		SkillDic = new()
		{
			["Fireball"] = Resources.Load<Fireball>("Skills/Fireball"),
		};

		// 스킬을 사용한다는건 오브젝트풀 사용
		// 오브젝트풀에서 사용할 오브젝트를 가져오고 플레이어의 스탯 * 스킬의 Damage계수 = 총대미지

        foreach (var skill in SkillDic.Values)
        {
            skill.IsCooldown = false;
        }
	}

	public SkillData GetSkill(string skillName) => SkillDic.TryGetValue(skillName, out SkillData skill) ? skill : null;


}
