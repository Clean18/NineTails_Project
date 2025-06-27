using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : BaseUI, IUI
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private TMP_Text hpTextt;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI warmthText;
    [SerializeField] private TextMeshProUGUI spritenergyText;
    private Upgrade upgrade;  // Test용으로 Upgrade에 있는 Test재화 사용할 변수
    private void Start()
    {
        //upgrade = FindObjectOfType<Upgrade>();
        // GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        // GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        // GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();

        UIManager.Instance.MainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
    }

    public void UIInit()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();
        GetEvent("Skill").Click += data => UIManager.Instance.ShowPopUp<SkillPopUp>();
        PlayerStatUI();

        // Text
        var data = GameManager.Instance.PlayerController?.PlayerModel.Data;
        powerText.text = $"Power : {data.PowerLevel}";
        attackText.text = $"Attack : {data.Attack}";
        defenseText.text = $"Defense : {data.Defense}";
        _hpSlider.value = (float)data.Hp / data.MaxHp;
        hpTextt.text = $"{data.Hp}/{data.MaxHp}";
    }

    // 메인에 플레이어 스탯 정보UI
    public void PlayerStatUI()
    {
        //powerText.text = $"Power Lev:{플레이어 최대체력}";
        //attackText.text = $"Attack:{플레이어 공격력}";
        //defenseText.text = $"Defense:{플레이어 방어력}";
        //hpText.text = $"{체력}/{최대 체력}";
        //spritenergyText.text = $"S: {영기재화}";
        //warmthText.text = $"W: {upgrade.Warmth}";
    }
}
