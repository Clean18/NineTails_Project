using UnityEngine;

public class StagePopUp : BaseUI
{
    [SerializeField] private string[] sceneNames;
    private void Start()
    {
        GetEvent("Btn_Stage11").Click += data =>
        {
            string missionId = "M1";               // 미션 아이디 > 클리어한 미션인지 체크용
            string sceneName = sceneNames[0];      // 해당 미션 씬 인덱서

            // 쿨타임이 돌때 미션 진행불가
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            //if (SceneManager.GetActiveScene().name != sceneName)
            //{
            //    SceneChangeManager.Instance.LoadSceneAsync(sceneName);
            //}
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            popUp.SetScene(sceneName, 4);      // 씬 이름, 저장할 씬 인덱스 전달
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            popUp.SetScene(sceneName, 8);      // 씬 이름, 저장할 씬 인덱스 전달
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 10);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 13);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 17);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 19);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 22);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 25);
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
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 28);
        };
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
    }

    void MissionCooldown()
    {
        Debug.Log("쿨타임 중 - 미션 재도전 불가");
        UIManager.Instance.ShowWarningText("쿨타임 중 - 미션 재도전 불가");
    }
}
