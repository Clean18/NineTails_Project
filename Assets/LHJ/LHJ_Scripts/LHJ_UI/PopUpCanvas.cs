using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpCanvas : MonoBehaviour
{
    private BaseUI current; // 현재 팝업


    public void ShowUI(BaseUI ui)
    {
        current = ui; // 현재 UI 등록
    }
    public void CloseUI()
    {
        if (current != null)
        {
            Destroy(current.gameObject); // 현재 팝업 UI 제거 
            current = null; 
        }
    }
}
