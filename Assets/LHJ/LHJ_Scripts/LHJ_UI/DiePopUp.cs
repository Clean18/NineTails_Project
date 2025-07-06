using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DiePopUp : BaseUI
{
    // 이동할 씬 배열로 저장
    [SerializeField] private string[] stageRespawn;
    [SerializeField] private string[] bossStage;
    [SerializeField] private TMP_Text descText;
    private void Start()
    {
        PlayerController.Instance.SaveData();
        //PlayerController.Instance.StartCoroutine(PreviousStage()); // 10초 후 자동 이동
        string currentScene = SceneManager.GetActiveScene().name;
        bool isBossStage = bossStage.Contains(currentScene); // 배열로 보스 스테이지 확인

        if (isBossStage)
        {
            descText.text = "사망하셨습니다.\n10초 후 이전 스테이지에서 자동으로 부활합니다.";
            PlayerController.Instance.StartCoroutine(SpecialPreviousStage());
        }
        else
        {
            descText.text = "사망하셨습니다.\n10초 후 현재 스테이지에서 자동으로 부활합니다.";
            PlayerController.Instance.StartCoroutine(CurrentStage()); // 10초 후 자동 이동
        }
        GetEvent("Btn_close").Click += data =>
        {
            //PreviousStageNow(); // 버튼 클릭 시 바로 이동
            UIManager.Instance.ClosePopUp();
        };
    }

    private IEnumerator CurrentStage()
    {
        yield return new WaitForSeconds(10f);
        SceneChangeManager.Instance.LoadCurrentScene();
    }
    private IEnumerator SpecialPreviousStage()
    {
        yield return new WaitForSeconds(10f); // 10초 대기

        string currentScene = SceneManager.GetActiveScene().name;
        int index = System.Array.IndexOf(stageRespawn, currentScene);   // 현재 씬 인덱스
        int previousIndex = Mathf.Max(0, index - 1);                    // 이전 인덱스
        string previousScene = stageRespawn[previousIndex];            // 이전 씬 이름

        SceneManager.LoadScene(previousScene);                         // 씬 이동
    }
}
