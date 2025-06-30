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
    [SerializeField] private TextMeshProUGUI timeText;          // 미션 시간
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
        GetEvent("Mission").Click += data => UIManager.Instance.ShowPopUp<StartMissionPopUp>();
        PlayerStatUI();

        var model = GameManager.Instance.PlayerController?.PlayerModel;
        model.Data.OnStatChanged += PlayerStatUI;
        model.Cost.OnCostChanged += PlayerStatUI;
    }

    // 메인에 플레이어 스탯 정보UI
    public void PlayerStatUI()
    {
        var model = GameManager.Instance.PlayerController?.PlayerModel;
        if (model == null) return;
        // Data
        powerText.text = $"Power : {model.Data?.PowerLevel}";
        attackText.text = $"Attack : {model.Data?.Attack}";
        defenseText.text = $"Defense : {model.Data?.Defense}";
        _hpSlider.value = (float)model.Data.Hp / model.Data.MaxHp;
        hpText.text = $"{model.Data?.Hp}/{model.Data?.MaxHp}";
        // Cost
        _warmthText.text = $"W: {model.Cost?.Warmth}";
        _spritenergyText.text = $"S: {model.Cost?.SpiritEnergy}";
    }
    private void Update()
    {
        // 미션이 실행 중이면 시간 표시
        if (MissionManager.Instance != null && MissionManager.Instance.IsRunning())
        {
            float time = MissionManager.Instance.GetRemainingTime();
            timeText.text = $"Time : {Mathf.CeilToInt(time)}s";  // 초 단위로 표시
        }
        else
        {
            timeText.text = "";
        }
    }
}
