using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiePopUp : BaseUI
{
    // 이동할 씬 배열로 저장
    [SerializeField] private string[] stageRespawn;
    private void Start()
    {
        PlayerController.Instance.SaveData();
        PlayerController.Instance.StartCoroutine(PreviousStage()); // 10초 후 자동 이동
        GetEvent("Btn_close").Click += data =>
        {
            //PreviousStageNow(); // 버튼 클릭 시 바로 이동
            UIManager.Instance.ClosePopUp();
        };
    }

    private IEnumerator PreviousStage()
    {
        yield return new WaitForSeconds(10f);
        SceneChangeManager.Instance.LoadCurrentScene();
        //PreviousStageNow(); // 자동 이동
    }
    private void PreviousStageNow()
    {
        //string currentScene = SceneManager.GetActiveScene().name;
        //int index = System.Array.IndexOf(stageRespawn, currentScene);   // 현재 씬 인덱스
        //int previousIndex = Mathf.Max(0, index - 1);    // 이전 인덱스 계산
        //string previousScene = stageRespawn[previousIndex]; // 이전 씬 인덱스

        //SceneManager.LoadScene(previousScene);  // 씬 이동 실행

        // 현재 씬 로드
        SceneChangeManager.Instance.LoadCurrentScene();
    }
}
