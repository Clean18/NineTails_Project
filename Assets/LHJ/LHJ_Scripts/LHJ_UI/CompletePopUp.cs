using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletePopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Btn_Y").Click += data =>
        {
            // 미션에서 다음 씬 이름 가져오기
            string nextScene = MissionManager.Instance.GetNextScene();
            SceneManager.LoadScene(nextScene);
        };
    }
}
