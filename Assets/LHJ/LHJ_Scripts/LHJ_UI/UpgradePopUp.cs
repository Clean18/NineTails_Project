using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradePopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI gradeText; // 등급 텍스트
    [SerializeField] private TextMeshProUGUI levelText; // 강화 단계 텍스트

    // 활성화 될때마다 호출되는 함수
    private void OnEnable() => UpdateText();

    private void Start()
    {
        Debug.LogError("UpgradePopUp 초기화");
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();

        // 강화 버튼 클릭시 Upgrade에 있는 TryEnhance 함수 실행 후 텍스트 업데이트
        GetEvent("Upgrade").Click += data =>
        {
            PlayerController.Instance.TryEnhance();
            UpdateText();
        };

        // 승급 버튼 클릭시 TryPromote 함수 실행 후 텍스트 업데이트
        GetEvent("Promotion").Click += data =>
        {
            PlayerController.Instance.TryPromote();
            UpdateText();
        };
    }

    // UI 텍스트 업데이트 함수
    public void UpdateText()
    {
        var equipmentData = PlayerController.Instance.GetEquipmentData();
        gradeText.text = $"Grade: {equipmentData.Grade}";
        levelText.text = $"Level: {equipmentData.Level}";
    }
}
