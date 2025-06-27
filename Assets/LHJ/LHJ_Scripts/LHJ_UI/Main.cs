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
    [SerializeField] private TMP_Text hpText;

    private void Start()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();

        UIManager.Instance.MainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
    }

    public void UIInit()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();

        // Text
        var data = GameManager.Instance.PlayerController?.PlayerModel.Data;
        powerText.text = $"Power : {data.PowerLevel}";
        attackText.text = $"Attack : {data.Attack}";
        defenseText.text = $"Defense : {data.Defense}";
        _hpSlider.value = (float)data.Hp / data.MaxHp;
        hpText.text = $"{data.Hp}/{data.MaxHp}";
    }
}
