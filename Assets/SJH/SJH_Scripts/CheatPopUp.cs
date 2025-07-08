using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatPopUp : BaseUI
{
    void Start()
    {
        Debug.Log("치트팝업 활성화");
        GetEvent("Btn_Y").Click += data =>
        {
            GameManager.IsCheat = true;
            UIManager.Instance.ClosePopUp();
            UIManager.Instance.MainUI.UIInit();
        };
        GetEvent("Btn_N").Click += data =>
        {
            UIManager.Instance.ClosePopUp();
        };
    }
}
