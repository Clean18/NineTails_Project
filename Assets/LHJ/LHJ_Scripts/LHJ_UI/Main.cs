using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Main : BaseUI
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    private void Start()
    {
        GetEvent("Equipment").Click += data => UIManager.Instance.ShowPopUp<UpgradePopUp>();
        GetEvent("Setting").Click += data => UIManager.Instance.ShowPopUp<SettingPopUp>();
        GetEvent("Stats").Click += data => UIManager.Instance.ShowPopUp<StatusPopUp>();
    }
}
