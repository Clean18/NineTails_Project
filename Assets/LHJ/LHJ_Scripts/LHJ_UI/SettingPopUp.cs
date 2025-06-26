using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopUp : BaseUI
{
    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();

        // Quit 버튼 클릭시 QuitPopUp 생성
        GetEvent("Quit").Click += data => UIManager.Instance.ShowPopUp<QuitPopUp>();
    }
}
