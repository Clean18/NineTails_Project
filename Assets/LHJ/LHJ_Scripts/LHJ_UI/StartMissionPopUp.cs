using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMissionPopUp : BaseUI
{
    private string _sceneName; // 씬 이름

    public void SetScene(string sceneName)
    {
        _sceneName = sceneName;
    }
    private void Start()
    {
        GetEvent("Btn_Y").Click += data => {
            UIManager.Instance.ClosePopUp();
            MissionManager.Instance.StartMission(_sceneName);
            SceneManager.LoadScene(_sceneName); // 씬 이동
        };
        GetEvent("Btn_N").Click += data => UIManager.Instance.ClosePopUp();
    }

}
