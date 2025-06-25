using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePopUp : BaseUI
{
    private void Start()
    {
        GetEvent("BackButton").Click += data => UIManager.Instance.ClosePopUp();
    }
}
