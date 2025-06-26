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

    // 활성화 될때마다 호출되는 함수
    private void OnEnable()
    {
        if (upgrade == null)                            // upgrade가 null일때
            upgrade = FindObjectOfType<Upgrade>();      // Upgrade컴포넌트가 붙어있는 오브젝트 참조
        if (promotion == null)                          // promotion이 null일때
            promotion = FindObjectOfType<Promotion>();  // Promotion컴포넌트가 붙어있는 오브젝트 참조
        UpdateText();
    }
    private void Start()
    {
        // Upgrade,Promotion 컴포넌트가 붙어있는 오브젝트 참조
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
