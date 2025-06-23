using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 테스트를 위해 임시로 생성한 Scene 전환 클래스입니다.
/// </summary>
public class SceneChangerManager_Test : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("CYH_GameScene");
    }
}
