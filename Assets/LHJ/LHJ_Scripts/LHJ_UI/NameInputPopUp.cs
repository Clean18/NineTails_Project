using TMPro;
using UnityEngine;

public class NameInputPopUp : BaseUI
{
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        Debug.Log("이름 생성 팝업 활성화");
        GetEvent("Btn_Confirm").Click += data =>
        {
            string name = inputField.text.Trim();
            Debug.Log($"플레이어 이름 : [{name}]");
            // 입력값이 비어있지 않을때
            if (!string.IsNullOrEmpty(name))
            {
                // 플레이어 이름 지정
                Debug.Log($"플레이어 이름 : [{name}] 으로 결정");
                PlayerController.Instance.SetPlayerName(name);
                PlayerController.Instance.SaveData();
                Time.timeScale = 1;
                UIManager.Instance.ClosePopUp(); // 현재 팝업 닫기
                UIManager.Instance.ShowPopUp<CompletePopUp>(); // 클리어 팝업 열기
            }
        };
        GetEvent("Btn_N").Click += data =>
        {
            PlayerController.Instance.SaveData();
            Time.timeScale = 1;
            UIManager.Instance.ClosePopUp();
            UIManager.Instance.ShowPopUp<CompletePopUp>(); // 클리어 팝업 열기
        };
    }
}
