using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 가진 스킬리스트를 관리하는 클래스입니다.
/// </summary>
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] private List<ActiveSkillData> _activeSkillData;
   
}