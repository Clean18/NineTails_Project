using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopUp : BaseUI
{
    private void Start()
    {
        // 뒤로가기 버튼 클릭시 팝업창 닫기
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp(); // BackButton

        // TODO : BGM 조절 슬라이더
        // TODO : 사운드 이펙트 조절 슬라이더
        // TODO : 대미지 텍스트
        // TODO : 스킬 이펙트

        // Quit 버튼 클릭시 QuitPopUp 생성
        GetEvent("End").Click += data => UIManager.Instance.ShowPopUp<QuitPopUp>(); // Quit
    }
}
