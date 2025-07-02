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
            /*스킬0레벨 증가*/;
            Debug.Log("스킬0 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill1Up").Click += data =>
        {
            /*스킬1레벨 증가*/;
            Debug.Log("스킬1 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill2Up").Click += data =>
        {
            /*스킬2레벨 증가*/;
            Debug.Log("스킬2 강화 버튼");
            UpdateSkill();
        };
        GetEvent("Skill3Up").Click += data =>
        {
            /*스킬3레벨 증가*/;
            Debug.Log("스킬3 강화 버튼");
            UpdateSkill();
        };
    }

    public void UpdateSkill()
    {
        //skill0levelText.text = $"Level:{스킬0 레벨}";
        //skill1levelText.text = $"Level:{스킬1 레벨}";
        //skill2levelText.text = $"Level:{스킬2 레벨}";
        //skill3levelText.text = $"Level:{스킬3 레벨}";
    }
}
