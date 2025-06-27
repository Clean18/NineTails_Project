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
        GetEvent("HpUpgrade").Click += data => { /*체력 스탯증가 함수*/; Debug.Log("체력 강화 버튼"); UpdateStatUpUI(); };
        GetEvent("AttackUpgrade").Click += data => { /*공격 스탯증가 함수*/; Debug.Log("공격력 강화 버튼"); UpdateStatUpUI(); };
        GetEvent("DefenseUpgrade").Click += data => { /*방어력 스탯증가 함수*/; Debug.Log("방어력 강화 버튼"); UpdateStatUpUI(); };
    }

    public void UpdateStatUpUI()
    {
        // 체력 레벨
        // hpLevelText.text = $"HP Lv:{플레이어 체력 레벨}";
        // hpCostText.text = $"Cost: {스탯강화 비용}";

        // 공격력 레벨과 다음 레벨업 비용
        // attackLevelText.text = $"Attack Lv:{플레이어 공격력 레벨}";
        // attackCostText.text = $"Cost: {스탯강화 비용} ";

        // 방어력 레벨
        // defenseLevelText.text = $"Defense Lv:{플레이어 방어력 레벨}";
        // defenseCostText.text = $"Cost: {스탯강화 비용} ";
    }
}
