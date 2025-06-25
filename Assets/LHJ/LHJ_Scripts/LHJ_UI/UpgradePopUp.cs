using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradePopUp : BaseUI
{
    private Upgrade upgrade;
    private Promotion promotion;

    [SerializeField] private TextMeshProUGUI gradeText; // 등급 텍스트
    [SerializeField] private TextMeshProUGUI levelText; // 강화 단계 텍스트
    private void Start()
    {
        upgrade = FindObjectOfType<Upgrade>();
        promotion = FindObjectOfType<Promotion>();
        
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();

        // 강화 버튼 클릭시 Upgrade에 있는 TryEnhance 함수 실행 후 텍스트 업데이트
        GetEvent("Upgrade").Click += data => { upgrade.TryEnhance(); UpdateText(); };

        // 승급 버튼 클릭시 TryPromote 함수 실행 후 텍스트 업데이트
        GetEvent("Promotion").Click += data => { upgrade.TryPromoteWith(promotion); UpdateText(); };
    }

    // UI 텍스트 업데이트 함수
    private void UpdateText()
    {
        gradeText.text = $"Grade: {upgrade.CurrentGrade}";
        levelText.text = $"Level: {upgrade.CurrentLevel}";
    }
}
