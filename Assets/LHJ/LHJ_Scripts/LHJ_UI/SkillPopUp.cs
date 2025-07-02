using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI skill0levelText;
    [SerializeField] private TextMeshProUGUI skill1levelText;
    [SerializeField] private TextMeshProUGUI skill2levelText;
    [SerializeField] private TextMeshProUGUI skill3levelText;

    private void Start()
    {
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();
        GetEvent("Skill0Up").Click += data =>
        {
            PlayerController.Instance.TrySkillLevelUp(0);
            Debug.Log("스킬0 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill1Up").Click += data =>
        {
            PlayerController.Instance.TrySkillLevelUp(1);
            Debug.Log("스킬1 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill2Up").Click += data =>
        {
            PlayerController.Instance.TrySkillLevelUp(2);
            Debug.Log("스킬2 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill3Up").Click += data =>
        {
            PlayerController.Instance.TrySkillLevelUp(3);
            Debug.Log("스킬3 강화 버튼");
            UpdateSkill();
        };
    }

    private void OnEnable() => UpdateSkill();

    public void UpdateSkill()
    {
        var skills = PlayerController.Instance.GetSkillData();
        if (skills == null || skills.Count < 1) return;
        skill0levelText.text = $"Level:{skills[0].SkillLevel}";
        skill1levelText.text = $"Level:{skills[1].SkillLevel}";
        skill2levelText.text = $"Level:{skills[2].SkillLevel}";
        skill3levelText.text = $"Level:{skills[3].SkillLevel}";
    }
}
