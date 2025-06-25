using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public StartUI StartUI;

    public GameUI GameUI;

    private PopUpCanvas popUpCanvas;
    public PopUpCanvas PopUpCanvas
    {
        get
        {   // 이미 팝업 캔버스가 있을때
            if (popUpCanvas != null)
                return popUpCanvas;

            // 팝업 캔버스가 없을때 씬에서 팝업캔버스 오브젝트 검색
            popUpCanvas = FindObjectOfType<PopUpCanvas>();
            if (popUpCanvas != null)
                return popUpCanvas;

            // 둘다 없을때 Resources에서 팝업캔버스 로드
            PopUpCanvas prefab = Resources.Load<PopUpCanvas>("PopUpCanvas");
            Debug.Log($"PopUpCanvas 로드 결과: {prefab}");
            return Instantiate(prefab);
        }
    }

    // BaseUI를 상속하고있는 팝업만 생성
    public T ShowPopUp<T>() where T : BaseUI
    {
        T prefab = Resources.Load<T>($"PopUp/{typeof(T).Name}");
        Debug.Log($"프리팹 로드 경로: PopUp/{typeof(T).Name}, 결과: {prefab}");

        if (prefab == null)
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: Resources/PopUp/{typeof(T).Name}");
            return null;
        }
        T instance = Instantiate(prefab, PopUpCanvas.transform);

        PopUpCanvas.ShowUI(instance);
        return instance;
    }

    // 현재 팝업 제거
    public void ClosePopUp()
    {
        PopUpCanvas.CloseUI();
    }
}
