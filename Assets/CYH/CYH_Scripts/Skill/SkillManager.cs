using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 가진 스킬을 관리 및 사용하는 클래스입니다.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private List<GameObject> _skillPrefabs;
    [SerializeField] private List<int> _skillLevels;
    private List<ISkill> _skillLogics = new List<ISkill>();         // 보유 중인 스킬 목록

    [SerializeField] private ISkill[] _hotkeys = new ISkill[3];     // 단축키에 등록된 스킬 목록


    private void Start()
    {
        for (int i = 0; i < _skillPrefabs.Count; i++)
        {
            GameObject skillPrefab = Instantiate(_skillPrefabs[i], transform);
            ISkill iSkill = skillPrefab.GetComponent<ISkill>();
            if (iSkill != null)

                _skillLogics.Add(iSkill);
        }
    }

    private void Update()
    {
        if (!Input.anyKeyDown)
            return;

        // 플레이어가 가진 스킬 목록을 돌며 키 체크 및 사용
        for (int i = 0; i < _skillLogics.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _skillLogics[i]?.UseSkill(transform);
            }
        }

        // 단축키에 등록된 스킬 목록을 돌며 키 체크 및 사용
        //for (int i = 0; i < _hotkeys.Length; i++)
        //{
        //    if (_hotkeys[i] == null)
        //        continue;

        //    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
        //    {
        //        _hotkeys[i].UseSkill(transform);
        //    }
        //}
    }

    /// <summary>
    /// 플레이어가 보유하고 있는 스킬(_skillLogics) 중에서 원하는 스킬을 단축키 슬롯에 등록
    /// skillIndex: _skillLogics 리스트 인덱스
    /// </summary>
    public void RegisterHotkey(int slot, int skillIndex)
    {

    }
}