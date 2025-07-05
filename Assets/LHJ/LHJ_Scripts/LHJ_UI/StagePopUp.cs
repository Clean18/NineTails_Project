using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePopUp : BaseUI
{
    [SerializeField] private string[] sceneNames;
    private void Start()
    {
        GetEvent("Btn_Stage11").Click += data =>
        {
            string missionId = "M1";               // 미션 아이디
            string sceneName = sceneNames[0];      // 해당 미션 씬 인덱서

            //if (!MissionManager.Instance.IsCleared(missionId))      // 클리어 된 상태가 아니라면
            //{
            //    if (MissionManager.Instance.IsCooldownActive)       // 쿨타임이 돌때 미션 진행불가
            //    {
            //        Debug.Log("쿨타임 중 - 미션 재도전 불가");
            //        return;
            //    }
            //    var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            //    popUp.SetScene(sceneName);      // 씬 이름 전달
            //}
            //else    // 클리어 상태인경우
            //{
            //    if (SceneManager.GetActiveScene().name == sceneName)
            //    {
            //        Debug.Log("현재 씬이 이동할 씬과 같은 씬입니다.");
            //    }
            //    else
            //    {
            //        SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            //    }
            //}
            if (MissionManager.Instance.IsCooldownActive)       // 쿨타임이 돌때 미션 진행불가
            {
                Debug.Log("쿨타임 중 - 미션 재도전 불가");
                return;
            }
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            popUp.SetScene(sceneName);      // 씬 이름 전달
        };

        GetEvent("Btn_Stage12").Click += data =>
        {
            string missionId = "M2";
            string sceneName = sceneNames[1];
            if (!MissionManager.Instance.IsCleared("M1"))
            {
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
                // TODO : 현재 씬과 비교해서 같은 씬이면 return
                if (SceneManager.GetActiveScene().name == sceneName)
                {
                    Debug.Log("현재 씬이 이동할 씬과 같은 씬입니다.");
                }
                else
                {
                    SceneChangeManager.Instance.LoadSceneAsync(sceneName);
                }
            }
        };
        GetEvent("Btn_Stage13").Click += data =>
        {
            string missionId = "M3";
            string sceneName = sceneNames[2];
            if (!MissionManager.Instance.IsCleared("M2"))
            {
                UIManager.Instance.ShowWarningText("1-2 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage21").Click += data =>
        {
            string missionId = "M4";
            string sceneName = sceneNames[3];
            if (!MissionManager.Instance.IsCleared("M3"))
            {
                UIManager.Instance.ShowWarningText("1-3 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage22").Click += data =>
        {
            string missionId = "M5";
            string sceneName = sceneNames[4];
            if (!MissionManager.Instance.IsCleared("M4"))
            {
                UIManager.Instance.ShowWarningText("2-1 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage23").Click += data =>
        {
            string missionId = "M6";
            string sceneName = sceneNames[5];
            if (!MissionManager.Instance.IsCleared("M5"))
            {
                UIManager.Instance.ShowWarningText("2-2 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage31").Click += data =>
        {
            string missionId = "M7";
            string sceneName = sceneNames[6];
            if (!MissionManager.Instance.IsCleared("M6"))
            {
                UIManager.Instance.ShowWarningText("2-3 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage32").Click += data =>
        {
            string missionId = "M8";
            string sceneName = sceneNames[7];
            if (!MissionManager.Instance.IsCleared("M7"))
            {
                UIManager.Instance.ShowWarningText("3-1 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_Stage33").Click += data =>
        {
            string missionId = "M9";
            string sceneName = sceneNames[8];
            if (!MissionManager.Instance.IsCleared("M8"))
            {
                UIManager.Instance.ShowWarningText("3-2 스테이지 클리어 이후 사용가능합니다.");
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
                SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            }
        };
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
    }
}
