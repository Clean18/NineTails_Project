using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopUp : BaseUI
{
    private void Start()
    {
        // Yes 버튼을 누르면 게임 종료
        GetEvent("Yes").Click += data => {/* 저장 함수*/; Application.Quit(); };
        GetEvent("No").Click += data => UIManager.Instance.ClosePopUp();
    }
}
