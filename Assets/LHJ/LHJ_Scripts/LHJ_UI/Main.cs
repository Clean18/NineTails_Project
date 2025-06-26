using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : BaseUI
{
    private void Start()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
    }
}
