using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 가진 스킬을 관리 및 사용하는 클래스입니다.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    //[SerializeField] private List<GameObject> _skillPrefabs;
    //public List<ISkill> SkillLogics = new List<ISkill>();         // 보유 중인 스킬 목록
    //private Dictionary<ISkill, int> _skillLevelDict = new Dictionary<ISkill, int>();

    //[SerializeField] private ISkill[] _hotkeys = new ISkill[3];     // 단축키에 등록된 스킬 목록

    //private void Start()
    //{
    //    for (int i = 0; i < _skillPrefabs.Count; i++)
    //    {
    //        GameObject skillPrefab = Instantiate(_skillPrefabs[i], transform);
    //        ISkill iSkill = skillPrefab.GetComponent<ISkill>();
    //        if (iSkill != null)
    //        {
    //            Debug.Log($"{_skillPrefabs[i].name} 스킬 추가");
    //            SkillLogics.Add(iSkill);
    //        }
    //    }
    //}
}