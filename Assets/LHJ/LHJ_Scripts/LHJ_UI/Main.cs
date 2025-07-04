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
    [SerializeField] private TextMeshProUGUI retrycoolTimeText; // 미션 재도전 남은시간
    private void Start()
    {
        UIManager.Instance.MainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
        Debug.Log($"Main 씬 UI 리스트에 추가 {UIManager.Instance.SceneUIList.Count}");
    }

    void OnEnable() => PlayerStatUI();

    // 플레이어가 초기화될 때 실행
    public void UIInit()
    {
        Debug.LogWarning("Main 초기화");
        GetEvent("Btn_Stat").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>(); // Stats
        GetEvent("Btn_Skill").Click += data => UIManager.Instance.ShowPopUp<SkillPopUp>(); // Skill
        GetEvent("Btn_Weapon").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>(); //Equipment
        GetEvent("Btn_Option").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>(); // Setting
        var mission = GetEvent("Mission");
        if (mission != null)
        {
            mission.Click += data => {
                if (MissionManager.Instance.IsCooldownActive)
                    return;
                UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            };
        }
        PlayerStatUI();
        GetEvent("Btn_Achievement").Click += data => UIManager.Instance.ShowPopUp<AchievementPopUp>(); // Achievement
        PlayerController.Instance.ConnectEvent(PlayerStatUI);
    }

    // 메인에 플레이어 스탯 정보UI
    public void PlayerStatUI()
    {
        var player = PlayerController.Instance;
        if (player == null) return;
        // Data
        powerText.text = $"Power : {player.GetPower()}";
        attackText.text = $"Attack : {player.GetAttack()}";
        defenseText.text = $"Defense : {player.GetDefense()}";
        // TODO : UI 전부 추가하면 지우기
        if (_hpSlider != null) _hpSlider.value = (float)player.GetHp() / player.GetMaxHp();
        hpText.text = $"{player.GetHp()}/{player.GetMaxHp()}";
        // Cost
        _warmthText.text = $"W: {player.GetWarmth()}";
        _spritenergyText.text = $"S: {player.GetSpiritEnergy()}";
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
            timeText.text = "";         // 미션 진행중이 아닐때 텍스트 초기화
        }

        // 쿨타임 진행중일때 재도전 남은 시간 표시
        if (MissionManager.Instance.IsCooldownActive)
        {
            float seconds = MissionManager.Instance.CooldownSeconds;
            if (retrycoolTimeText != null) retrycoolTimeText.text = $"RetryCoolTime: {Mathf.CeilToInt(seconds)}s";  // 남은 쿨타임 표시
        }
        else
        {
            if (retrycoolTimeText != null) retrycoolTimeText.text = "";    // 쿨타임 끝나면 텍스트 초기화
        }
    }
}
