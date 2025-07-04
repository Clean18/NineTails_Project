using TMPro;
using UnityEngine;

public class UpgradePopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI _currentGradeText; // 등급, 단계 텍스트

    [SerializeField] private TMP_Text _nextGradeText;       // 다음 강화 등급 텍스트
    [SerializeField] private TMP_Text _costText;            // 강화 비용 텍스트
    [SerializeField] private TMP_Text _upgradeBtnText;      // 강화 버튼 텍스트


    [SerializeField] private TMP_Text _attackText;          // 현재 공격력 % 증가
    [SerializeField] private TMP_Text _cooldownText;        // 스킬 쿨타임 감소
    [SerializeField] private TMP_Text _reduceDamageText;    // 받는피해감소 관통
    [SerializeField] private TMP_Text _increaseDamageText;  // 가하는 피해 증가


    private void Start()
    {
        Debug.LogWarning("UpgradePopUp 초기화");
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp(); // BackButton

        // 강화 버튼 클릭시 Upgrade에 있는 TryEnhance 함수 실행 후 텍스트 업데이트
        GetEvent("UpgradeBtn").Click += data =>
        {
            PlayerController.Instance.TryEnhance();
            UpdateText();
        };

        // 승급 버튼 클릭시 TryPromote 함수 실행 후 텍스트 업데이트
        GetEvent("PromotionBtn").Click += data =>
        {
            PlayerController.Instance.TryPromote();
            UpdateText();
        };
    }

    // 활성화 될때마다 호출되는 함수
    private void OnEnable() => UpdateText();

    // UI 텍스트 업데이트 함수
    public void UpdateText()
    {
        var equipmentData = PlayerController.Instance.GetEquipmentData();
        long cost = DataManager.Instance.GetEquipmentUpgradeCost(PlayerController.Instance.GetGradeType(), equipmentData.Level);
        var currentData = DataManager.Instance.GetEquipmentUpgradeInfo(PlayerController.Instance.GetGradeType(), equipmentData.Level);
        var nextData = DataManager.Instance.GetEquipmentUpgradeInfo(PlayerController.Instance.GetGradeType(), equipmentData.Level + 1);

        _currentGradeText.text = $"현재 등급 : {GradeEngToKor(equipmentData.Grade)}\n현재 레벨 : {equipmentData.Level}";

        // TODO : 무기 등급에 따라 텍스트 바뀜
        string nextGradeText = "";
        switch (equipmentData.Grade)
        {
            case "N": nextGradeText = $"공격력\n{currentData.Attack * 100}% → {nextData.Attack * 100}%"; break;
            case "R": nextGradeText = $"스킬 쿨타임 감소\n{currentData.CooldownReduction * 100}% → {nextData.CooldownReduction * 100}%"; break;
            case "SR": nextGradeText = $"받는 피해 감소 관통\n{currentData.ReduceDamage * 100}% → {nextData.ReduceDamage * 100}%"; break;
            case "SSR": nextGradeText = $"가하는 피해\n{PlayerController.Instance.GetIncreseDamage(equipmentData.Level) * 10}% → {PlayerController.Instance.GetIncreseDamage(equipmentData.Level + 1) * 10}%"; break;
        }

        _nextGradeText.text = nextGradeText;

        _costText.text = $"강화 비용\n{cost}";

        if (equipmentData.Grade == "SSR")
        {
            _attackText.text = $"공격력 50% 증가";
            _cooldownText.text = $"스킬 쿨타임 감소 30%";
            _reduceDamageText.text = $"받는 피해 감소 관통 30% 증가";
            _increaseDamageText.text = $"가하는 피해 {PlayerController.Instance.GetIncreseDamage() * 10}% 증가";
        }
        else
        {
            _attackText.text = $"공격력 {currentData.Attack * 100}% 증가";
            _cooldownText.text = $"스킬 쿨타임 감소 {currentData.CooldownReduction * 100}%";
            _reduceDamageText.text = $"받는 피해 감소 관통 {currentData.ReduceDamage * 100}% 증가";
            _increaseDamageText.text = $"가하는 피해 {PlayerController.Instance.GetIncreseDamage() * 10}% 증가";
        }

        var promotionBtn = GetEvent("PromotionBtn");
        if (promotionBtn != null)
        {
            promotionBtn.gameObject.SetActive(equipmentData.Level == 50);
        }
    }

    string GradeEngToKor(string eng)
    {
        switch (eng)
        {
            case "R": return "고급";
            case "SR": return "보물";
            case "SSR": return "설화";
            default: return "평범";
        }
    }
}
