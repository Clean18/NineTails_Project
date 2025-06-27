using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI maxHpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();
        GetEvent("Stats Up").Click += data => UIManager.Instance.ShowPopUp<StatUpPopUp>();
        UpdateStatUI();
    }

    // 플레이어의 현재 스탯 텍스트
    public void UpdateStatUI()
    {
        //maxHpText.text = $"MaxHp:{플레이어 최대체력}";
        //attackText.text = $"Attack:{플레이어 공격력}";
        //defenseText.text = $"Defense:{플레이어 방어력}";
    }
}
