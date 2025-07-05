using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMissionPopUp : BaseUI
{
    private string _sceneName; // 씬 이름
    [SerializeField] private TMP_Text desc;

    public void SetScene(string sceneName)
    {
        _sceneName = sceneName;
    }
    private void Start()
    {
        if (DataManager.Instance.MissionTable.TryGetValue(_sceneName, out MissionInfo currentMission))
        {
            if (MissionManager.Instance.MissionIds.Contains(currentMission.Id))
            {
                // 미션 꺤 텍스트
                desc.text = "다음 스테이지에 넘어가기 위해\n돌파 미션을 진행하시겠습니까?";
            }
            else
            {
                // 기본 텍스트
                desc.text = "아직 클리어하지 않은 스테이지입니다.\n돌파 미션을 진행하시겠습니까?";
            }
        }

        GetEvent("Btn_Y").Click += data => {
            UIManager.Instance.ClosePopUp();
            MissionManager.Instance.StartMission(_sceneName); // 해당 씬에 해당되는 미션 시작
            //SceneManager.LoadScene(_sceneName); // 씬 이동

            // TODO : 로딩씬이름 하드코딩 변경
            SceneChangeManager.Instance.LoadSceneWithLoading("LoadingScene_v1", _sceneName, 1);
        };
        GetEvent("Btn_N").Click += data => UIManager.Instance.ClosePopUp();
    }
}
