using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 가진 스킬을 관리 및 사용하는 클래스입니다.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private List<GameObject> _skillPrefabs;
    
    public List<ISkill> SkillLogics = new List<ISkill>();                               // 보유 중인 스킬 목록
    private Dictionary<ISkill, int> _skillLevelDict = new Dictionary<ISkill, int>();    // 스킬/레벨 딕셔너리

    [SerializeField] private ISkill[] _hotkeys = new ISkill[3];                         // 단축키에 등록된 스킬 목록


    private void Start()
    {
        for (int i = 0; i < _skillPrefabs.Count; i++)
        {
            GameObject skillPrefab = Instantiate(_skillPrefabs[i], transform);
            ISkill iSkill = skillPrefab.GetComponent<ISkill>();
            if (iSkill != null)
            {
                SkillLogics.Add(iSkill);
                _skillLevelDict.Add(iSkill, iSkill.SkillLevel);
                Debug.Log($"{iSkill}, {iSkill.SkillLevel}");
            }
        }
    }

    private void Update()
    {
        // 스킬 레벨 테스트
        if (Input.GetKeyDown(KeyCode.Alpha5))
            IncreaseSkillLevel(0);

        if (Input.GetKeyDown(KeyCode.Alpha6))
            IncreaseSkillLevel(1);
    }

    // 스킬 레벨 상승
    public void IncreaseSkillLevel(int skillIndex)
    {
        // 범위 체크
        if (skillIndex < 0 || skillIndex >= SkillLogics.Count)
            return;

        // 유효성 체크
        ISkill iSkill = SkillLogics[skillIndex];
        if (!_skillLevelDict.ContainsKey(iSkill))
            return;

        _skillLevelDict[iSkill]++;
        Debug.Log($"{iSkill} 레벨 1 상승 → 레벨 {_skillLevelDict[iSkill]}");
    }
}