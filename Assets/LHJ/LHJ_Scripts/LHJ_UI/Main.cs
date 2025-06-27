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
    [SerializeField] private TextMeshProUGUI warmthText;
    [SerializeField] private TextMeshProUGUI spritenergyText;
    private Upgrade upgrade;  // Test용으로 Upgrade에 있는 Test재화 사용할 변수
    private void Start()
    {
        upgrade = FindObjectOfType<Upgrade>();
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();
        GetEvent("Skill").Click += data => UIManager.Instance.ShowPopUp<SkillPopUp>();
        PlayerStatUI();
    }

    // 메인에 플레이어 스탯 정보UI
    public void PlayerStatUI()
    {
        //powerText.text = $"Power Lev:{플레이어 최대체력}";
        //attackText.text = $"Attack:{플레이어 공격력}";
        //defenseText.text = $"Defense:{플레이어 방어력}";
        //hpText.text = $"{체력}/{최대 체력}";
        //spritenergyText.text = $"S: {영기재화}";
        warmthText.text = $"W: {upgrade.Warmth}";
    }
}
