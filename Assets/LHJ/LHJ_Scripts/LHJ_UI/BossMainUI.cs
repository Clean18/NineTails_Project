using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossMainUI : BaseUI, IUI
{
    [SerializeField] private TextMeshProUGUI powerText;         // 전투력
    [SerializeField] private TextMeshProUGUI attackText;        // 공격력
    [SerializeField] private TextMeshProUGUI defenseText;       // 방어력
    [SerializeField] private Slider _hpSlider;                  // 체력바
    [SerializeField] private Slider _shieldSlider;              // 실드바
    [SerializeField] private TMP_Text hpText;                   // 체력 / 최대체력
    [SerializeField] private TextMeshProUGUI timeText;          // 미션 시간
    [SerializeField] private Transform bossStageCamera;        // 카메라가 위치할 오브젝트
    [SerializeField] private float cameraSize;
    [SerializeField] private BossHpUI bossHpUI;
    [SerializeField] private BaseBossFSM storyBoss;
    [SerializeField] private BaseBossFSM missionBoss;

    private void Start()
    {
        UIManager.Instance.BossMainUI = this;
        UIManager.Instance.SceneUIList.Add(this);
        Debug.Log($"Main 씬 UI 리스트에 추가 {UIManager.Instance.SceneUIList.Count}");
    }

    public void UIInit()
    {
        Debug.LogWarning("BossMain 초기화");
      
        GetEvent("Btn_Option").Click += data => // Setting
        {
            Debug.Log("옵션 UI 활성화");
            UIManager.Instance.ShowPopUp<SettingPopUp>();
        };
        bool isMission = MissionManager.Instance != null && MissionManager.Instance.IsRunning();

        BaseBossFSM selectedBoss;

        if (isMission)
        {
            missionBoss.gameObject.SetActive(true);
            storyBoss.gameObject.SetActive(false);
            selectedBoss = missionBoss;
        }
        else
        {
            missionBoss.gameObject.SetActive(false);
            storyBoss.gameObject.SetActive(true);
            selectedBoss = storyBoss;
        }

        bossHpUI.Init(selectedBoss);

        PlayerStatUI();
        PlayerController.Instance.ConnectEvent(PlayerStatUI);
        SetupCameraForBossStage();
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
    }
    private void SetupCameraForBossStage()
    {
        Camera cam = Camera.main;
        if (bossStageCamera != null && cam != null)
        {
            cam.transform.SetParent(null); // 플레이어에서 분리
            cam.transform.position = bossStageCamera.position;
            cam.transform.rotation = bossStageCamera.rotation;
            cam.orthographicSize = cameraSize;
        }
    }
}
