using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedPopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Retry").Click += data =>
        {
            UIManager.Instance.ClosePopUp();
            MissionManager.Instance.StartMission(); // 팝업창을 닫고 미션 재시작
        };
        GetEvent("Close").Click += data => UIManager.Instance.ClosePopUp();
    }
}
