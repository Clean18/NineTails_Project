using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatUpPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI hpLevelText;
    [SerializeField] private TextMeshProUGUI attackLevelText;
    [SerializeField] private TextMeshProUGUI defenseLevelText;

    [SerializeField] private TextMeshProUGUI hpCostText;
    [SerializeField] private TextMeshProUGUI attackCostText;
    [SerializeField] private TextMeshProUGUI defenseCostText;
    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();
        GetEvent("HpUpgrade").Click += data =>
        {
            PlayerController.Instance.TryHpLevelup();
            Debug.Log("체력 강화 버튼");
            UpdateStatUpUI();
        };
        GetEvent("AttackUpgrade").Click += data =>
        {
            PlayerController.Instance.TryAttackLevelup();
            Debug.Log("공격력 강화 버튼");
            UpdateStatUpUI();
        };
        GetEvent("DefenseUpgrade").Click += data =>
        {
            PlayerController.Instance.TryDefenseLevelup();
            Debug.Log("방어력 강화 버튼");
            UpdateStatUpUI();
        };
    }

    void OnEnable() => UpdateStatUpUI();

    public void UpdateStatUpUI()
    {
        var data = PlayerController.Instance.SaveData();
        // 체력 레벨
        hpLevelText.text = $"HP Lv:{data.HpLevel}";
        hpCostText.text = $"Cost: {"StatUpgrade Cost"}"; // 스탯 강화 비용

        // 공격력 레벨과 다음 레벨업 비용
        attackLevelText.text = $"Attack Lv:{data.AttackLevel}";
        attackCostText.text = $"Cost: {"StatUpgrade Cost"}";

        // 방어력 레벨
        defenseLevelText.text = $"Defense Lv:{data.DefenseLevel}";
        defenseCostText.text = $"Cost: {"StatUpgrade Cost"}";
    }
}
