using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillUI
{
	public Image icon;
	public TMP_Text descText;
	public TMP_Text costText;
	public Button upgradeButton;
	public Button getButton;
	public Button Active;
}

public class SkillPopUp : BaseUI
{
	[SerializeField] private List<SkillUI> skillUIList;
	/// <summary>
	/// Table[skillIndex] = "설명 텍스트"
	/// </summary>
	private Dictionary<int, string> skillDescDic = new()
	{
		// TODO : 설명들어오면 설명대로 변경
		[0] = $"0검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 1타 100% > 101% 2타 50% > 50.5%
		[1] = $"1검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 75% > 75.75%
		[2] = $"2검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 25% > 25.25% 실드도 똑같음
		[3] = $"3검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 100% > 101%
		[4] = $"4검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 15% > 15.15% / 힐 5% > 5.05% / 0.05f + 0.0005f * SkillLevel
		[5] = $"5검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 12% > 12.12% / 0.12f + 0.0012f * SkillLevel
		[6] = $"6검에 혼령의 힘을 담아 주변의 적에게 <color=#FF0000>{0}%</color> -> <color=#FF0000>{1}%</color>의 피해를 입힌다.", // 400% > 404% / 
	};
	//private Dictionary<int, >

	private void Start()
	{
		GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp(); // BackButton

		// 스킬 강화 버튼 클릭 이벤트 일괄 등록
		for (int i = 0; i < skillUIList.Count; i++)
		{
			int skillIndex = i;
			skillUIList[i].upgradeButton.onClick.AddListener(() =>
			{
				PlayerController.Instance.TrySkillLevelUp(skillIndex);
				Debug.Log($"스킬 {skillIndex} 강화 버튼 클릭됨");
				UpdateSkill();
			});
			skillUIList[i].getButton.onClick.AddListener(() =>
			{
				// TODO : 스킬 획득
				PlayerController.Instance.AddSkill(skillIndex);
				Debug.Log($"스킬 {skillIndex} 습득 버튼 클릭됨");
				UpdateSkill();
			});
            // TODO : Active 버튼 기능 추가
		}
		//GetEvent("Skill0Up").Click += data =>
		//{
		//    PlayerController.Instance.TrySkillLevelUp(0);
		//    Debug.Log("스킬0 강화 버튼");
		//    UpdateSkill();
		//};
	}

	private void OnEnable() => UpdateSkill();

	public void UpdateSkill()
	{
		var skills = PlayerController.Instance.GetSkillData();
		if (skills == null || skills.Count < 1) return;

		// 리스트 합치기
		List<ISkill> hasSkills = new();
		hasSkills.AddRange(PlayerController.Instance.GetSkillMappingList());
		hasSkills.AddRange(PlayerController.Instance.GetHasSkillList());

		// 리스트 SkillIndex로 정렬
		hasSkills.Sort((a, b) => a.SkillData.SkillIndex.CompareTo(b.SkillData.SkillIndex));
		var allSkills = PlayerController.Instance.SkillController.SkillList;

		// 배열 에러나서 딕셔너리로 변경
		Dictionary<int, ISkill> hasSkillDic = new();
		foreach (var skill in hasSkills)
		{
			int index = skill.SkillData.SkillIndex;
			if (!hasSkillDic.ContainsKey(index)) hasSkillDic.Add(index, skill);
		}

		for (int i = 0; i < allSkills.Count; i++)
		{
			var ui = skillUIList[i];

			// 스킬 보유중이 아님
			if (!hasSkillDic.TryGetValue(i, out ISkill skillData))
			{
				// 스킬컨트롤러에서 기본 정보 받아오기
				// 아이콘
				ui.icon.sprite = allSkills[i].SkillData.SkillSprite;
				// 설명
				ui.descText.text = allSkills[i].SkillData.Description;
				// 강화 비용
				ui.costText.text = $"강화 비용";

				// 습득 버튼 상태 설정
				ui.getButton.gameObject.SetActive(true);
			}
			// 스킬 보유중
			else
			{
				// 아이콘
				ui.icon.sprite = skillData.SkillData.SkillSprite;
				// 설명
				ui.descText.text = string.Format(
					skillDescDic[i],
					$"{GetSkillDamage(i, skillData.SkillLevel) * 100:F2}",
					$"{GetSkillDamage(i, skillData.SkillLevel + 1) * 100:F2}"
					);
				// 강화 비용
				string costText = $"강화 비용\n{(skillData.SkillData.SkillIndex == 6 ? DataManager.Instance.GetUltSkillCost(skillData.SkillLevel) : DataManager.Instance.GetNormalSkillCost(skillData.SkillLevel))}";
				ui.costText.text = skillData.SkillLevel == 100 ? "강화 완료" : costText;

				// 습득 버튼 상태 설정
				ui.getButton.gameObject.SetActive(false);
			}

		}
	}

	float GetSkillDamage(int skillIndex, int skillLevel)
	{
		/*  스킬 0
			1타 100% > 101%
			1f + 0.01f * SkillLevel
			2타 50% > 50.5%
			0.5f + 0.005f * SkillLevel

			스킬 1
			75% > 75.75%
			0.75f + 0.0075f * SkillLevel

			스킬 2
			25% > 25.25% 딜, 힐 똑같음
			0.25f + 0.0025f * SkillLevel

			스킬 3
			100% > 101%
			1f + 0.01f * SkillLevel

			스킬 4
			15% > 15.15%
			0.15f + 0.0015f * SkillLevel
			힐 5% > 5.05%
			0.05f + 0.0005f * SkillLevel

			스킬 5
			12% > 12.12%
			0.12f + 0.0012f * SkillLevel

			스킬 6
			400% > 404%
			4f + 0.04f * SkillLevel
		 */
		float damage = 1;
		switch (skillIndex)
		{
			case 0: damage = 1f + 0.01f * skillLevel; break;
			case 1: damage = 0.75f + 0.0075f * skillLevel; break;
			case 2: damage = 0.25f + 0.0025f * skillLevel; break;
			case 3: damage = 1f + 0.01f * skillLevel; break;
			case 4: damage = 0.15f + 0.0015f * skillLevel; break;
			case 5: damage = 0.12f + 0.0012f * skillLevel; break;
			case 6: damage = 4f + 0.04f * skillLevel; break;
		}
		return damage;
	}
}