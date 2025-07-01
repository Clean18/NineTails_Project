using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _skillPrefabs = new();
    public List<ISkill> SkillList = new();

    public void InitSkillController()
    {
        foreach (var prefab in _skillPrefabs)
        {
            var go = Instantiate(prefab, PlayerController.Instance.SkillController.transform);
            var sl = go.GetComponent<ISkill>();
            SkillList.Add(sl);
        }
        Debug.Log($"스킬 컨트롤러 초기화 완료 총 스킬 개수 : {SkillList.Count}");
    }
}
