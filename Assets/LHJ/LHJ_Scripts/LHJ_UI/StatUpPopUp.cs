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
        
    }
}
