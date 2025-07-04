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
    [SerializeField] private TMP_Text _soulText;                // 혼백
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
        // 여기서 버튼들 팝업 활성화
        GetEvent("Btn_Stat").Click += data => // Stats
        {
            Debug.Log("스탯 강화 UI 활성화");
            UIManager.Instance.ShowPopUp<StatUpPopUp>(); // StatusPopUp
        };
        GetEvent("Btn_Skill").Click += data => // Skill
        {
            Debug.Log("스킬 강화 UI 활성화");
            UIManager.Instance.ShowPopUp<SkillPopUp>();
        };
        GetEvent("Btn_Weapon").Click += data => //Equipment
        {
            Debug.Log("장비 강화 UI 활성화");
            UIManager.Instance.ShowPopUp<UpgradePopUp>();
        };
        GetEvent("Btn_Option").Click += data => // Setting
        {
            Debug.Log("옵션 UI 활성화");
            UIManager.Instance.ShowPopUp<SettingPopUp>();
        };
        //GetEvent("Mission").Click += data => // Mission
        //{
        //    if (MissionManager.Instance.IsCooldownActive)
        //        return;
        //    UIManager.Instance.ShowPopUp<StartMissionPopUp>();
        //};
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
        powerText.text = $"전투력 : {player.GetPower()}";
        attackText.text = $"공격력 : {player.GetAttack()}";
        defenseText.text = $"방어력 : {player.GetDefense()}";
        // TODO : UI 전부 추가하면 지우기
        if (_hpSlider != null) _hpSlider.value = (float)player.GetHp() / player.GetMaxHp();
        double hpPer = (double)player.GetHp() / player.GetMaxHp() * 100f;
        hpText.text = $"{hpPer:F0}%";
        // Cost
        _warmthText.text = $"{player.GetWarmth()}";
        _spritenergyText.text = $"{player.GetSpiritEnergy()}";
        // 플레이어 스킬이 7개가 아니면 활성화
        _soulText.transform.parent.gameObject.SetActive(PlayerController.Instance.GetSkillData().Count != 7);
        _soulText.text = $"{player.GetSoul()}";
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
