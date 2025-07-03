using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMissionPopUp : BaseUI
{
    private void Start()
    {
        // Yes 버튼을 누르면 게임 종료
        GetEvent("Yes").Click += data => {
            UIManager.Instance.ClosePopUp();
            MissionManager.Instance.StartMission();  // 미션 시작
        };

        GetEvent("No").Click += data => UIManager.Instance.ClosePopUp();
    }
}
