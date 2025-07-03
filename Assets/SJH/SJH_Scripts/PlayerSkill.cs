using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveSkillData
{
    /// <summary>
    /// 스킬 번호
    /// </summary>
	public int SkillIndex;
    /// <summary>
    /// 스킬 레벨
    /// </summary>
	public int SkillLevel;
    /// <summary>
    /// 슬롯 번호
    /// </summary>
	public int SlotIndex;
}

[System.Serializable]
public class PlayerSkill
{
    // 단축키
	public Dictionary<KeyCode, ISkill> SkillMapping;
    public List<ISkill> HasSkills;
	private SkillController _controller;
	public ISkill DefaultAttack => SkillMapping[KeyCode.Mouse0];
	//public SkillLogic_1 Skill1;
	//public SkillLogic_2 Skill2;

	// 슬롯인덱스 키코드 변환 딕셔너리
	private readonly Dictionary<KeyCode, int> KeyCodeToSlotIndexDic = new()
	{
		{ KeyCode.Mouse0, 0 },
		{ KeyCode.Alpha1, 1 },
		{ KeyCode.Alpha2, 2 },
		{ KeyCode.Alpha3, 3 }
	};
	private readonly Dictionary<int, KeyCode> SlotIndexToKeyCodeDic = new()
	{
		{ 0, KeyCode.Mouse0 },
		{ 1, KeyCode.Alpha1 },
		{ 2, KeyCode.Alpha2 },
		{ 3, KeyCode.Alpha3 }
	};


	public void InitSkill(List<SaveSkillData> skillDatas)
	{
		_controller = PlayerController.Instance.SkillController;

        //if (_controller.SkillList[0] is SkillLogic_0_HitBox skill0) DefaultAttack = skill0;
        //if (_controller.SkillList[1] is SkillLogic_1 skill1) Skill1 = skill1;
        //if (_controller.SkillList[2] is SkillLogic_2 skill2) Skill2 = skill2;

        // 가진 스킬
        HasSkills = new();
        // 단축키
        SkillMapping = new()
        {
            [KeyCode.Mouse0] = _controller.SkillList[0],
            [KeyCode.Alpha1] = null,
            [KeyCode.Alpha2] = null,
            [KeyCode.Alpha3] = null,
        };

		// 기본공격 미리 추가

		foreach (var data in skillDatas)
		{
			if (data.SkillIndex < 0) continue;

			// 스킬컨트롤러의 프리팹 순서는 0 기본공격 1~6 스킬순서 맞춰야함
			var skill = _controller.SkillList[data.SkillIndex];
			skill.SkillLevel = data.SkillLevel;

			KeyCode key = SlotIndexToKeyCode(data.SlotIndex);
			// 기본공격 제외 추가
			if (key != KeyCode.Mouse0 && data.SkillIndex != 0) SkillMapping[key] = skill;
		}
		Debug.Log("플레이어 스킬 초기화 완료");
	}

	public ISkill GetSkill(KeyCode keyCode) => SkillMapping.TryGetValue(keyCode, out ISkill value) ? value : null;

    /// <summary>
    /// 스킬 저장 함수
    /// </summary>
    /// <returns></returns>
	public List<SaveSkillData> SavePlayerSkill()
	{
		List<SaveSkillData> saveSkills = new();

		// 단축키 스킬 세이브
		foreach (var pair in SkillMapping)
		{
			KeyCode key = pair.Key;
			ISkill skill = pair.Value;

			if (skill == null) continue;

			int slotIndex = KeyCodeToSlotIndex(key);
			int skillLevel = skill.SkillLevel;

			saveSkills.Add(new SaveSkillData
			{
				SlotIndex = slotIndex,
				SkillIndex = skill.SkillData.SkillIndex,
				SkillLevel = skillLevel
			});

			// 매핑에 없는 슬롯 인덱스는 -1
		}
        // TODO : 가지고 있는 스킬 세이브
        // 테스트 해봐야함
        foreach (var skill in HasSkills)
        {
            saveSkills.Add(new SaveSkillData
            {
                SlotIndex = -1,
                SkillIndex = skill.SkillData.SkillIndex,
                SkillLevel = skill.SkillLevel
            });
        }

		return saveSkills;
	}

	int KeyCodeToSlotIndex(KeyCode key) => KeyCodeToSlotIndexDic.TryGetValue(key, out int result) ? result : -1;
	KeyCode SlotIndexToKeyCode(int index) => SlotIndexToKeyCodeDic.TryGetValue(index, out KeyCode result) ? result : KeyCode.None;

    /// <summary>
    /// 스킬을 추가하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void AddSkill(int skillIndex)
    {
        if (_controller.SkillList.Count < 1 || _controller.SkillList.Count < skillIndex)
        {
            Debug.Log("스킬이 없습니다.1");
            return;
        }

        ISkill newSkill = _controller.SkillList[skillIndex];

        if (newSkill == null)
        {
            Debug.Log("스킬이 없습니다.2");
            return;
        }

        // TODO : 스킬 중복체크
        if (SkillMapping.ContainsValue(newSkill) || HasSkills.Contains(newSkill))
        {
            Debug.Log($"{newSkill.SkillData.SkillName} 스킬은 이미 가지고 있는 스킬입니다.");
            return;
        }

        // 스킬 추가
        bool isSlotAdd = false;
        KeyCode key = KeyCode.None;
        // 1. 단축키 추가
        foreach (var pair in SkillMapping)
        {
            if (pair.Value != null || isSlotAdd) continue;

            isSlotAdd = true;
            key = pair.Key;
            // 슬롯 인덱스 변경
            newSkill.SlotIndex = KeyCodeToSlotIndex(key);
            // 스킬레벨 0으로
            newSkill.SkillLevel = 0;

            Debug.Log($"{newSkill.SlotIndex} 슬롯에 {newSkill.SkillData.SkillName} 스킬 획득");
        }

        if (isSlotAdd) SkillMapping[key] = newSkill;

        // 2. 스킬리스트 추가
        else if (!isSlotAdd)
        {
            HasSkills.Add(newSkill);
            Debug.Log($"{newSkill.SkillData.SkillName} 스킬 획득");
        }

    }
}