using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedPopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
    }
}
