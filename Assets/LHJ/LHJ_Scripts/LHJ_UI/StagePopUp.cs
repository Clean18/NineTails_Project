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
            string missionId = "M1";                // 미션 아이디
            string sceneName = "Stage1-1Copy";      // 해당 미션 씬 이름

            if (!MissionManager.Instance.IsCleared(missionId))      // 클리어 된 상태가 아니라면
            {
                if (MissionManager.Instance.IsCooldownActive)       // 쿨타임이 돌때 미션 진행불가
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
                var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
                popUp.SetScene(sceneName);      // 씬 이름 전달
            }
            else    // 클리어 상태인경우
            {
                // TODO : 현재 씬과 비교해서 같은 씬이면 return
                SceneManager.LoadScene(sceneName);  // 바로 해당씬으로 이동
            }
        };

        GetEvent("Btn_Stage12").Click += data =>
        {
            string missionId = "M2";
            string sceneName = "Stage1-2Test";

            if (!MissionManager.Instance.IsCleared("M1"))
            {
                // TODO : 이전 미션 클리어 안했으면 경고문 출력
                UIManager.Instance.ShowWarningText("1-1 스테이지 클리어 이후 사용가능합니다.");
                return;
            }

            if (!MissionManager.Instance.IsCleared(missionId))
            {
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
                if (MissionManager.Instance.IsCooldownActive)
                {
                    Debug.Log("쿨타임 중 - 미션 재도전 불가");
                    return;
                }
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
