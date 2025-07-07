using UnityEngine;

public class CompletePopUp : BaseUI
{
    private void Start()
    {
        GetEvent("Btn_Y").Click += data =>
        {
            // 미션에서 다음 씬 이름 가져오기
            string nextScene = MissionManager.Instance.GetNextScene();
            //SceneManager.LoadScene(nextScene);

            int index = PlayerController.Instance.GetPlayerSceneIndex();
            switch (index)
            {
                case 4:
                    Debug.Log($"4 > 6번 씬으로 이동");
                    PlayerController.Instance.SetPlayerSceneIndex(6);
                    SceneChangeManager.Instance.LoadCurrentScene();
                    break; // 4 > 6
                case 13:
                    Debug.Log($"13 > 15번 씬으로 이동");
                    PlayerController.Instance.SetPlayerSceneIndex(15);
                    SceneChangeManager.Instance.LoadCurrentScene();
                    break; // 4 > 6
            }
            Debug.Log($"돌파미션 클리어! [{nextScene}] 씬으로 이동");
            //Debug.LogError($"{nextScene}");
            SceneChangeManager.Instance.LoadNextScene();
            //SceneChangeManager.Instance.LoadSceneWithLoading("LoadingScene_v1", nextScene, 1);
        };
        GetEvent("Btn_N").Click -= data =>
        {
            UIManager.Instance.ClosePopUp();
        };
    }
}
