using UnityEngine;

public class SettingPopUp : BaseUI
{
    [SerializeField] private AudioSettingsUI _forRef;

    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp(); // BackButton

        // BGM 조절 슬라이더
        // 사운드 이펙트 조절 슬라이더
        // _forRef 에서 처리

        // 대미지 텍스트
        GetEvent("Damagetext").Click += data =>
        {
            UIManager.IsFloatingText = !UIManager.IsFloatingText;
        };

        // Quit 버튼 클릭시 QuitPopUp 생성
        GetEvent("End").Click += data => UIManager.Instance.ShowPopUp<QuitPopUp>(); // Quit
    }
}
