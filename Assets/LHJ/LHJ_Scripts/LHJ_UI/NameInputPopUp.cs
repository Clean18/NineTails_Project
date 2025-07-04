using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInputPopUp : BaseUI
{
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        GetEvent("Btn_Confirm").Click += data =>
        {
            string name = inputField.text.Trim();

            // 입력값이 비어있지 않을때
            if (!string.IsNullOrEmpty(name))
            {
                // Ui매니저에 플레이어 이름 저장
                UIManager.Instance.PlayerName = name;
                UIManager.Instance.MainUI.UpdateNicknameUI(); // Main 닉네임 텍스트 UI갱신

                UIManager.Instance.ClosePopUp(); // 현재 팝업 닫기
                UIManager.Instance.ShowPopUp<CompletePopUp>(); // 클리어 팝업 열기
            }
        };
    }
}
