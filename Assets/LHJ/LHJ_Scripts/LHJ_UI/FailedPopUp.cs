using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedPopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Close").Click += data => UIManager.Instance.ClosePopUp();
    }
}
