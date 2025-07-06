using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

    [SerializeField] private List<SaveSkillData> skillList; // 직렬화용

	public void InitSkill(List<SaveSkillData> skillDatas = null)
	{
		_controller = PlayerController.Instance.SkillController;

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
        
        // 첫 시작
        if (skillDatas == null) return;

        // 단축키 초기화
        foreach (var data in skillDatas)
		{
			if (data.SkillIndex < 0) continue;

			// 스킬컨트롤러의 프리팹 순서는 0 기본공격 1~6 스킬순서 맞춰야함
			var skill = _controller.SkillList[data.SkillIndex];
			skill.SkillLevel = data.SkillLevel;

			KeyCode key = SlotIndexToKeyCode(data.SlotIndex);

            // 기본공격 제외 추가
            if (key != KeyCode.Mouse0 && data.SkillIndex != 0 && key != KeyCode.None)
            {
                SkillMapping[key] = skill;
                skill.SlotIndex = data.SlotIndex;
            }
            // 남은 스킬 추가
            else
            {
                if (data.SkillIndex == 0) continue;
                skill.SlotIndex = -1;
                HasSkills.Add(skill);
            }

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
        Debug.LogWarning($"저장할 스킬 개수 : {saveSkills.Count}");
        skillList = saveSkills;
		return saveSkills;
	}
    /// <summary>
    /// KeyCode > SlotIndex
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
	int KeyCodeToSlotIndex(KeyCode key) => KeyCodeToSlotIndexDic.TryGetValue(key, out int result) ? result : -1;
    /// <summary>
    /// SlotIndex > KeyCode
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
	KeyCode SlotIndexToKeyCode(int index) => SlotIndexToKeyCodeDic.TryGetValue(index, out KeyCode result) ? result : KeyCode.None;
    /// <summary>
    /// 스킬을 추가하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void AddSkill(int skillIndex)
    {
        if (_controller.SkillList.Count < 1 || _controller.SkillList.Count < skillIndex)
        {
            Debug.Log("스킬컨트롤러에 스킬 프리팹이 없습니다.1");
            return;
        }

        ISkill newSkill = _controller.SkillList[skillIndex];

        if (newSkill == null)
        {
            Debug.Log("스킬 인덱스에 맞는 스킬이 없습니다.");
            return;
        }

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
        SkillButton.Instance.UIInit();
    }
    /// <summary>
    /// skillIndex 스킬 레벨업을 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void TrySkillLevelUp(int skillIndex)
    {
        var player = PlayerController.Instance;
        var skill = player.SkillController.SkillList[skillIndex];
        if (skill == null)
        {
            Debug.Log("스킬이 없습니다.");
            return;
        }
        // 레벨 체크
        if (skill.SkillLevel >= 100)
        {
            Debug.Log("스킬 레벨을 더 이상 올릴 수 없습니다.");
            return;
        }
        // 재화 체크
        // TODO : 스킬이 노말인지 궁극기인지 구분해야함
        bool isUlt = skill.SkillData.SkillIndex == 6;

        if (isUlt) // 궁극기 강화
        {
            var ultCost = DataManager.Instance.GetUltSkillCost(skill.SkillLevel);
            if (ultCost == int.MaxValue)
            {
                Debug.Log("스킬을 강화할 수 없습니다.");
                return;
            }
            // 노말체크
            if (player.GetSpiritEnergy() < ultCost)
            {
                Debug.Log("영기가 부족합니다.");
                return;
            }
            // 재화 감소
            player.SpendCost(CostType.SpiritEnergy, ultCost);
        }
        else // 노말 강화
        {
            var normalCost = DataManager.Instance.GetNormalSkillCost(skill.SkillLevel);
            if (normalCost == int.MaxValue)
            {
                Debug.Log("스킬을 강화할 수 없습니다.");
                return;
            }
            if (player.GetSpiritEnergy() < normalCost)
            {
                Debug.Log("영기가 부족합니다.");
                return;
            }
            player.SpendCost(CostType.SpiritEnergy, normalCost);
        }
        /*
         * 플레이어의 스킬 보유 여부를 체크안한다면
         * UI창에서는 가지고 있는 스킬만 활성화하는 방식
         */
        skill.SkillLevel += 1;
        Debug.Log($"스킬 레벨업! : {skill.SkillData.SkillName} Lv. {skill.SkillLevel}");
    }
    /// <summary>
    /// 스킬 단축창에 등록되어 있는 스킬을 List로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<ISkill> GetSkillMappingList()
    {
        var list = new List<ISkill>();
        foreach (var skill in SkillMapping.Values)
        {
            if (skill != null) list.Add(skill);
        }
        return list;
    }
    /// <summary>
    /// skillIndex 번째 스킬을 획득하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    /// <param name="soul"></param>
    public void LearnSkill(int skillIndex, long soul)
    {
        // 중복 체크
        var skill = PlayerController.Instance.SkillController.SkillList[skillIndex];
        if (skill == null)
        {
            Debug.Log("배울 수 없는 스킬입니다.");
            return;
        }
        var mapping = GetSkillMappingList();
        var has = HasSkills;

        foreach (var mappingSkill in mapping)
        {
            if (mappingSkill.SkillData.SkillIndex == skill.SkillData.SkillIndex)
            {
                Debug.Log("이미 배운 스킬입니다.");
                return;
            }
        }
        foreach (var mappingSkill in has)
        {
            if (mappingSkill.SkillData.SkillIndex == skill.SkillData.SkillIndex)
            {
                Debug.Log("이미 배운 스킬입니다.");
                return;
            }
        }

        // 플레이어 재화 체크
        if (soul < 1 && !GameManager.IsCheat)
        {
            Debug.Log("혼백이 부족합니다.");
            return;
        }

        // 재화 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Soul, 1);

        // 스킬 추가
        AddSkill(skillIndex);
        // 버튼 업데이트
        SkillButton.Instance.UpdateButtonImage();
    }
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에 추가를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void AddSkillSlot(int skillIndex)
    {
        var mapping = SkillMapping;
        var has = HasSkills;
        ISkill skill = null;

        // 추가할 스킬이 가지고 있는 스킬인지 체크
        bool isHas = false;
        for (int i = 0; i < has.Count; i++)
        {
            if (has[i].SkillData.SkillIndex == skillIndex)
            {
                skill = has[i];
                isHas = true;
                break;
            }
        }

        if (skill == null || isHas == false)
        {
            Debug.Log("가지고 있지 않은 스킬입니다.");
            return;
        }

        if (mapping.ContainsValue(skill))
        {
            Debug.Log("이미 등록된 스킬입니다.");
            return;
        }

        KeyCode[] keyList = new KeyCode[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3
        };

        for (int i = 0; i < keyList.Length; i++)
        {
            KeyCode key = keyList[i];
            if (mapping.TryGetValue(key, out ISkill value) && value == null)
            {
                // 단축키 등록
                skill.SlotIndex = i + 1;
                mapping[key] = skill;
                // hasSkill에서 삭제
                has.Remove(skill);
                Debug.Log($"{skill.SkillData.SkillName} 스킬 {skill.SlotIndex}번 슬롯에 등록");
                return;
            }
        }

        Debug.Log("등록 가능한 슬롯이 없습니다.");
    }
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에서 제거를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void RemoveSkillSlot(int skillIndex)
    {
        var mapping = SkillMapping;
        var has = HasSkills;

        if (skillIndex < 1 || skillIndex > 6) return;

        ISkill skill = null;

        // 슬롯 삭제
        foreach (var pair in mapping)
        {
            // 단축키에서 스킬인덱스와 같은 스킬 찾기
            if (pair.Value != null && pair.Value.SkillData.SkillIndex == skillIndex)
            {
                skill = pair.Value;
                mapping[pair.Key] = null;
                break;
            }
        }

        if (skill == null)
        {
            Debug.Log($"SkillIndex : {skillIndex} 스킬은 등록된 스킬이 아닙니다.");
        }

        // 삭제되면 has리스트에 추가
        if (!has.Contains(skill))
        {
            var debug = skill.SlotIndex;
            Debug.Log($"{skill.SkillData.SkillName} 스킬 {debug}번 슬롯에서 제거");
            skill.SlotIndex = -1;
            has.Add(skill);
        }
    }
}