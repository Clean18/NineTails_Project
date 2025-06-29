using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 가진 스킬을 관리하는 클래스입니다.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private GameObject skill2Prefab;

    private ISkill _skill2Logic;

    private void Start()
    {
        // Skill_2 프리팹을 인스턴스화하고, 그 안의 SkillLogic_2를 참조
        GameObject skill2Instance = Instantiate(skill2Prefab, transform);
        _skill2Logic = skill2Instance.GetComponent<ISkill>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _skill2Logic?.UseSkill(transform);
        }
    }
}