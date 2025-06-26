using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Main : BaseUI
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI hpText;
    private void Start()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();
        GetEvent("Skill").Click += data => UIManager.Instance.ShowPopUp<SkillPopUp>();
        PlayerStatUI();
    }

    // 메인에 플레이어 스탯 정보UI
    private void PlayerStatUI()
    {
        //powerText.text = $"Power Lev:{플레이어 최대체력}";
        //attackText.text = $"Attack:{플레이어 공격력}";
        //defenseText.text = $"Defense:{플레이어 방어력}";
        //hpText.text = $"{체력}/{최대 체력}";
    }
}
