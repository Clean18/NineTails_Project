using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopUp : BaseUI
{
    private void Start()
    {
        // Yes 버튼을 누르면 게임 종료
        GetEvent("Btn_Y").Click += data =>
        {
            Debug.Log("게임 종료");
            SaveLoadManager.Instance.PlayerSave();
            Application.Quit();
        };
        GetEvent("Btn_N").Click += data =>
        {
            Debug.Log("게임 종료 취소");
            UIManager.Instance.ClosePopUp();
        };
    }
}
