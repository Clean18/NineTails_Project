using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePopUp : BaseUI
{
    [SerializeField] private string[] sceneNames;
    static bool firstSceneCheck = false;

    [SerializeField] private Image[] _sceneImages;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (i >= _sceneImages.Length) break; // 방어 코드

            if (sceneNames[i] == currentScene) _sceneImages[i].color = _selectedColor;
            else _sceneImages[i].color = _defaultColor;
        }

        GetEvent("Btn_Stage11").Click += data =>
        {
            if (!PlayerController.Instance.GetFirstWarmth())
            {
                Debug.Log("첫 온정 획득씬 안보면 돌파미션 수락 불가");
                UIManager.Instance.ShowWarningText("구미호님 너무 급하신거 아닌가용?????");
                firstSceneCheck = true;
                return;
            }
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
            // 쿨타임이 돌때 미션 진행불가
            if (MissionManager.Instance.IsCooldownActive)
            {
                MissionCooldown();
                return;
            }
            //string missionId = "M1";               // 미션 아이디 > 클리어한 미션인지 체크용
            string sceneName = sceneNames[0];      // 해당 미션 씬 인덱서
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            popUp.SetScene(sceneName, 4);      // 씬 이름, 저장할 씬 인덱스 전달
        };

        GetEvent("Btn_Stage12").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M2";
            string sceneName = sceneNames[1];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();  // 미션 팝업창 생성
            popUp.SetScene(sceneName, 8);      // 씬 이름, 저장할 씬 인덱스 전달
        };
        GetEvent("Btn_Stage13").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M3";
            string sceneName = sceneNames[2];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 10);
        };
        GetEvent("Btn_Stage21").Click += data =>
        {
            if (!PlayerController.Instance.GetFirstSpiritEnergy())
            {
                Debug.Log("첫 영기 획득씬 안보면 돌파미션 수락 불가");
                if (firstSceneCheck) UIManager.Instance.ShowWarningText("그렇게 급하시면 어제 출발하지 그랬슈~~");
                else UIManager.Instance.ShowWarningText("구미호님 너무 급하신거 아닌가용?????");
                return;
            }
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M4";
            string sceneName = sceneNames[3];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 13);
        };
        GetEvent("Btn_Stage22").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M5";
            string sceneName = sceneNames[4];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 17);
        };
        GetEvent("Btn_Stage23").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M6";
            string sceneName = sceneNames[5];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 19);
        };
        GetEvent("Btn_Stage31").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M7";
            string sceneName = sceneNames[6];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 22);
        };
        GetEvent("Btn_Stage32").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M8";
            string sceneName = sceneNames[7];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 25);
        };
        GetEvent("Btn_Stage33").Click += data =>
        {
            if (MissionManager.Instance.IsRunning())
            {
                Debug.Log("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                UIManager.Instance.ShowWarningText("돌파 미션중에는 스테이지를 이동할 수 없습니다.");
                return;
            }
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
            //string missionId = "M9";
            string sceneName = sceneNames[8];
            var popUp = UIManager.Instance.ShowPopUp<StartMissionPopUp>();
            popUp.SetScene(sceneName, 28);
        };
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
    }

    void MissionCooldown()
    {
        Debug.Log("돌파미션 쿨타임중 [아직 돌파 미션에 재도전할 수 없습니다.]");
        UIManager.Instance.ShowWarningText("아직 돌파 미션에 재도전할 수 없습니다.");
    }
}
