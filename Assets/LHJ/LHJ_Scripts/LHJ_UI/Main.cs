using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : BaseUI, IUI
{
    [SerializeField] private TextMeshProUGUI powerText;         // 전투력
    [SerializeField] private TextMeshProUGUI attackText;        // 공격력
    [SerializeField] private TextMeshProUGUI defenseText;       // 방어력
    [SerializeField] private Slider _hpSlider;                  // 체력바
    [SerializeField] private TMP_Text hpText;                   // 체력 / 최대체력
    [SerializeField] private TextMeshProUGUI _warmthText;       // 온기
    [SerializeField] private TextMeshProUGUI _spritenergyText;  // 영기
    private void Start()
    {
        UIManager.Instance.MainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
        Debug.Log($"Main 씬 UI 리스트에 추가 {UIManager.Instance.SceneUIList.Count}");
    }

    // 플레이어가 초기화될 때 실행
    public void UIInit()
    {
        Debug.LogError("Main 초기화");
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();
        GetEvent("Skill").Click += data => UIManager.Instance.ShowPopUp<SkillPopUp>();
        //GetEvent("Mission").Click += data => UIManager.Instance.ShowPopUp<StartMissionPopUp>();
        PlayerStatUI();

        PlayerController.Instance.ConnectEvent(PlayerStatUI);
    }

    // 메인에 플레이어 스탯 정보UI
    public void PlayerStatUI()
    {
        var player = PlayerController.Instance;
        // Data
        powerText.text = $"Power : {player.GetPower()}";
        attackText.text = $"Attack : {player.GetAttack()}";
        defenseText.text = $"Defense : {player.GetDefense()}";
        _hpSlider.value = (float)player.GetHp() / player.GetMaxHp();
        hpText.text = $"{player.GetHp()}/{player.GetMaxHp()}";
        // Cost
        _warmthText.text = $"W: {player.GetWarmth()}";
        _spritenergyText.text = $"S: {player.GetSpiritEnergy()}";
    }
}
