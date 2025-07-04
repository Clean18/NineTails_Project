using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Btn_Stage11").Click += data =>
        {
            string missionId = "M1";
            string sceneName = "Stage1-1Test";

            if (!MissionManager.Instance.IsCleared(missionId))      // 클리어 된 상태가 아니라면
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
                popUp.SetScene(sceneName);      // 씬 이름 전달
            }
            else    // 클리어 상태인경우
            {
                SceneManager.LoadScene(sceneName);  // 바로 해당씬으로 이동
            }
        };

        GetEvent("Btn_Stage12").Click += data =>
        {
            string missionId = "M3";
            string sceneName = "Stage1-2Test";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage13").Click += data =>
        {
            string missionId = "M3";
            string sceneName = "Stage1-3";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage21").Click += data =>
        {
            string missionId = "M4";
            string sceneName = "Stage2-1";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage22").Click += data =>
        {
            string missionId = "M5";
            string sceneName = "Stage2-2";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage23").Click += data =>
        {
            string missionId = "M6";
            string sceneName = "Stage2-3";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage31").Click += data =>
        {
            string missionId = "M7";
            string sceneName = "Stage3-1";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage32").Click += data =>
        {
            string missionId = "M8";
            string sceneName = "Stage3-2";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_Stage33").Click += data =>
        {
            string missionId = "M9";
            string sceneName = "Stage3-3";

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
                popUp.SetScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        };
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
    }
}
