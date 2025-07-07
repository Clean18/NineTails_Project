using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUpPopUp : BaseUI
{
    [SerializeField] private TMP_Text _playerName;  // 닉네임
    [SerializeField] private TMP_Text _power;       // 전투력

    [SerializeField] private Image _characterImage; // TODO : 캐릭터?

    [SerializeField] private TextMeshProUGUI _hpLevelText;
    [SerializeField] private TextMeshProUGUI _hpCostText;

    [SerializeField] private TextMeshProUGUI _attackLevelText;
    [SerializeField] private TextMeshProUGUI _attackCostText;

    [SerializeField] private TextMeshProUGUI _defenseLevelText;
    [SerializeField] private TextMeshProUGUI _defenseCostText;

    [SerializeField] private TextMeshProUGUI _speedLevelText;
    [SerializeField] private TextMeshProUGUI _speedCostText;

    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp(); // BackButton
        // 업그레이드 버튼은 여기서 연결
        GetEvent("HpUpgradeBtn").Click += data =>
        {
            PlayerController.Instance.TryHpLevelup();
            Debug.Log("체력 강화 버튼");
            UpdateStatUpUI();
        };
        GetEvent("AttackUpgradeBtn").Click += data =>
        {
            PlayerController.Instance.TryAttackLevelup();
            Debug.Log("공격력 강화 버튼");
            UpdateStatUpUI();
        };
        GetEvent("DefenseUpgradeBtn").Click += data =>
        {
            PlayerController.Instance.TryDefenseLevelup();
            Debug.Log("방어력 강화 버튼");
            UpdateStatUpUI();
        };
        GetEvent("SpeedUpgradeBtn").Click += data =>
        {
            PlayerController.Instance.TrySpeedLevelup();
            Debug.Log("이동속도 강화 버튼");
            UpdateStatUpUI();
        };
    }

    void OnEnable() => UpdateStatUpUI();

    public void UpdateStatUpUI()
    {
        var data = PlayerController.Instance.SaveData();
        long hpCost = DataManager.Instance.GetStatCost(StatDataType.Hp, data.HpLevel);
        long atkCost = DataManager.Instance.GetStatCost(StatDataType.Attack, data.AttackLevel);
        long defCost = DataManager.Instance.GetStatCost(StatDataType.Defense, data.DefenseLevel);
        long spdCost = DataManager.Instance.GetStatCost(StatDataType.Speed, data.SpeedLevel);

        // 플레이어 닉네임
        _playerName.text = $"닉네임 : {data.PlayerName}";
        // 플레이어 전투력
        _power.text = $"전투력 : {PlayerController.Instance.GetPower()}";

        // 체력
        _hpLevelText.text = $"체력 : {data.HpLevel} → {data.HpLevel + 1}";
        _hpCostText.text = $"강화비용\n{hpCost}"; // 스탯 강화 비용

        // 공격력
        _attackLevelText.text = $"공격력 : {data.AttackLevel} → {data.AttackLevel + 1}";
        _attackCostText.text = $"강화비용\n{atkCost}";

        // 방어력
        _defenseLevelText.text = $"방어력 : {data.DefenseLevel} → {data.DefenseLevel + 1}";
        _defenseCostText.text = $"강화비용\n{defCost}";

        // 이동속도
        _speedLevelText.text = $"이동속도 : {data.SpeedLevel} → {data.SpeedLevel + 1}";
        _speedCostText.text = $"강화비용\n{spdCost}";
    }
}
