using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 스킬들을 프리팹으로 초기화하는 스크립트
/// <br/> Player
/// <br/> ㄴSkillController
/// <br/> ㄴㄴSkill Prefab
/// <br/> ㄴㄴㄴHitbox
/// </summary>
public class SkillController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _skillPrefabs = new();
    public List<ISkill> SkillList = new();

    public void InitSkillController()
    {
        foreach (var prefab in _skillPrefabs)
        {
            // 플레이어의 스킬 컨트롤러 자식들에 각 스킬의 프리팹들 추가
            var go = Instantiate(prefab, PlayerController.Instance.SkillController.transform);
            var sl = go.GetComponent<ISkill>();
            SkillList.Add(sl);

            // 각 스킬 초기화
            sl.SkillInit();
        }
        Debug.Log($"스킬 컨트롤러 초기화 완료 총 스킬 개수 : {SkillList.Count}");
    }
}
