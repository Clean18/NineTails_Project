using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : BaseUI, IUI
{
    [SerializeField] private TextMeshProUGUI powerText;         // 전투력
    [SerializeField] private TextMeshProUGUI attackText;        // 공격력
    [SerializeField] private TextMeshProUGUI defenseText;       // 방어력
    [SerializeField] private Slider _hpSlider;                  // 체력바
    [SerializeField] private Slider _shieldSlider;              // 실드바
    [SerializeField] private TMP_Text hpText;                   // 체력 / 최대체력
    [SerializeField] private TextMeshProUGUI _warmthText;       // 온기
    [SerializeField] private TextMeshProUGUI _spritenergyText;  // 영기
    [SerializeField] private TMP_Text _soulText;                // 혼백
    [SerializeField] private TextMeshProUGUI timeText;          // 미션 시간
    [SerializeField] private TextMeshProUGUI retrycoolTimeText; // 미션 재도전 남은시간

    [SerializeField] private TMP_Text txt_Nickname;             // 닉네임 텍스트
    [SerializeField] private TextMeshProUGUI killCountText;     // 처치 수 / 목표 수
    [SerializeField] private Slider missionTimeSlider;          // 시간 슬라이드

    [SerializeField] private TMP_Text _autoBtnText;             // 자동/수동 버튼 텍스트

    private void Start()
    {
        UIManager.Instance.MainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
        Debug.Log($"Main 씬 UI 리스트에 추가 {UIManager.Instance.SceneUIList.Count}");
    }

    //void OnEnable()
    //{
    //    PlayerStatUI();
    //    UpdateNicknameUI();
    //}

    // 플레이어가 초기화될 때 실행
    public void UIInit()
    {
        Debug.LogWarning("Main 초기화");

        // 여기서 버튼들 팝업 활성화
        // 스탯 팝업
        GetEvent("Btn_Stat").Click += data => // Stats
        {
            Debug.Log("스탯 강화 UI 활성화");
            UIManager.Instance.ShowPopUp<StatUpPopUp>(); // StatusPopUp
        };
        // 스킬 팝업
        GetEvent("Btn_Skill").Click += data => // Skill
        {
            Debug.Log("스킬 강화 UI 활성화");
            UIManager.Instance.ShowPopUp<SkillPopUp>();
        };
        // 장비 팝업
        GetEvent("Btn_Weapon").Click += data => //Equipment
        {
            // 1-3 스테이지 클리어 업적 체크
            if ((AchievementManager.Instance.AchievedIds.ContainsKey("A3") && AchievementManager.Instance.AchievedIds["A3"]) || GameManager.IsCheat)
            {
                Debug.Log("장비 강화 UI 활성화");
                UIManager.Instance.ShowPopUp<UpgradePopUp>();
            }
            else
            {
                UIManager.Instance.ShowWarningText("1-3 스테이지 클리어 이후 사용가능합니다.");
            }
        };
        // 옵션 팝업
        GetEvent("Btn_Option").Click += data => // Setting
        {
            Debug.Log("옵션 UI 활성화");
            UIManager.Instance.ShowPopUp<SettingPopUp>();
        };
        // 스테이지 팝업
        GetEvent("Btn_Stage").Click += data => // Mission
        {
            UIManager.Instance.ShowPopUp<StagePopUp>();
        };
        // 치트 팝업
        var cheatBtn = GetEvent("Btn_Cheat");
        if (GameManager.IsCheat)
        {
            cheatBtn.gameObject.SetActive(false);
        }
        else
        {
            cheatBtn.Click += data =>
            {
                UIManager.Instance.ShowPopUp<CheatPopUp>();
            };
        }
        // 업적 팝업
        GetEvent("Btn_Achievement").Click += data => // Achievement
        {
            UIManager.Instance.ShowPopUp<AchievementPopUp>();
        };
        // 오토모드 팝업
        _autoBtnText.text = PlayerController.Instance.Mode == ControlMode.Auto ? "자동" : "수동";
        GetEvent("Btn_Auto").Click += data =>
        {
            if (GameManager.Instance.Player == null) return;

            // 보스방이면 오토 사용 불가
            string curSceneName = SceneManager.GetActiveScene().name;
            if (curSceneName == "Stage1-3_Battle" || curSceneName == "Stage2-3_Battle" || curSceneName == "Stage3-3_Battle") return;

            // 플레이어 모드 전환
            var player = PlayerController.Instance;

            player.Mode = player.Mode == ControlMode.Auto ? ControlMode.Manual : ControlMode.Auto;
            _autoBtnText.text = player.Mode == ControlMode.Auto ? "자동" : "수동";

            player.AIInit();

            // 플레이어 velocity 초기화
            player.AIStop();
        };
        // 오프라인 보상 팝업
        GetEvent("Btn_Offline2").Click += data =>
        {
            Debug.Log("오프라인 보상 팝업 활성화");
            UIManager.Instance.ShowPopUp<OfflineRewardPopUp>();
        };

        PlayerStatUI();
        PlayerController.Instance.ConnectEvent(PlayerStatUI);

        UpdateNicknameUI();
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

        _hpSlider.value = (float)player.GetHp() / player.GetMaxHp();
        double hpPer = (double)player.GetHp() / player.GetMaxHp() * 100f;
        hpText.text = $"{hpPer:F0}%";

        double shieldPer = (float)player.GetShieldHp() / player.GetMaxHp();
        _shieldSlider.value = (float)shieldPer;
        // Cost
        _warmthText.text = $"{player.GetWarmth()}";
        _spritenergyText.text = $"{player.GetSpiritEnergy()}";
        // 플레이어 스킬이 7개가 아니면 활성화
        _soulText.transform.parent.gameObject.SetActive(PlayerController.Instance.GetSkillData().Count != 7);
        _soulText.text = $"{player.GetSoul()}";
    }

    // 닉네임 갱신
    public void UpdateNicknameUI()
    {
        // 현재 플레이어 이름값
        string playerName = PlayerController.Instance.GetPlayerName();
        if (string.IsNullOrEmpty(playerName))
        {
            txt_Nickname.text = "구미호";
        }
        else
        {
            // 플레이어가 입력한 이름을 표시
            txt_Nickname.text = playerName;
        }
    }
    private void Update()
    {
        // 미션이 실행 중이면 시간 표시
        if (MissionManager.Instance != null && MissionManager.Instance.IsRunning())
        {
            float time = MissionManager.Instance.GetRemainingTime();
            timeText.text = $"Time : {Mathf.CeilToInt(time)}s";  // 초 단위로 표시

            int currentKill = MissionManager.Instance.killCount;
            int goal = DataManager.Instance.MissionTable[SceneManager.GetActiveScene().name].Count;
            killCountText.text = $"{currentKill} / {goal}";  // 처치 수 / 목표 수
            float total = DataManager.Instance.MissionTable[SceneManager.GetActiveScene().name].TimeLimit;
            missionTimeSlider.value = time / total;         // 제한시간감소 / 미션시간
            missionTimeSlider.gameObject.SetActive(true);
        }
        else
        {
            timeText.text = "";         // 미션 진행중이 아닐때 텍스트 초기화
            killCountText.text = "";
            missionTimeSlider.gameObject.SetActive(false);
        }

        // 쿨타임 진행중일때 재도전 남은 시간 표시
        if (MissionManager.Instance.IsCooldownActive)
        {
            float seconds = MissionManager.Instance.CooldownSeconds;
            if (retrycoolTimeText != null) retrycoolTimeText.text = $"Retry: {Mathf.CeilToInt(seconds)}s";  // 남은 쿨타임 표시
        }
        else
        {
            if (retrycoolTimeText != null) retrycoolTimeText.text = "";    // 쿨타임 끝나면 텍스트 초기화
        }
    }
}
