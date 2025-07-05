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
                    PlayerController.Instance.SetPlayerSceneIndex(6);
                    SceneChangeManager.Instance.LoadCurrentScene();
                    break;
            }

            //Debug.LogError($"{nextScene}");
            //SceneChangeManager.Instance.LoadSceneWithLoading("LoadingScene_v1", nextScene, 1);
        };
        GetEvent("Btn_N").Click -= data =>
        {
            UIManager.Instance.ClosePopUp();
        };
    }
}
