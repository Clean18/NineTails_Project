using UnityEngine;

public class NextSceneButton : MonoBehaviour
{
    // 버튼의 OnClick()에 연결
    public void OnNextButtonClicked()
    {
        SceneChangeManager.Instance.LoadNextScene();
    }
}