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
        {
            if (popUpCanvas != null)
                return popUpCanvas;

            popUpCanvas = FindObjectOfType<PopUpCanvas>();
            if (popUpCanvas != null)
                return popUpCanvas;

            PopUpCanvas prefab = Resources.Load<PopUpCanvas>("PopUpCanvas");
            return Instantiate(prefab);
        }
    }

    public T ShowPopUp<T>() where T : BaseUI
    {
        T prefab = Resources.Load<T>($"PopUp/{typeof(T).Name}");
        T instance = Instantiate(prefab, PopUpCanvas.transform);

        PopUpCanvas.ShowUI(instance);
        return instance;
    }

    public void ClosePopUp()
    {
        PopUpCanvas.CloseUI();
    }
}
