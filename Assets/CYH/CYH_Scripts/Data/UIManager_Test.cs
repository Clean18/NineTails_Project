using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임종료 버튼 클릭 시 실행 될 데이터 저장 기능을 테스트하기 위한 테스트용 UIManager입니다.
/// UIManager 클래스가 구현된 후 아래 내용 옮길 예정
/// </summary>
public class UIManager_Test : MonoBehaviour
{
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _quitButton.onClick.AddListener(() => GameEvents.RequestSave());
    }
}
