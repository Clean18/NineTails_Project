using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public StartUI StartUI;
    public GameUI GameUI;
    public Main MainUI;

    // 플레이어가 입력한 닉네임 저장 변수
    public string PlayerName { get; set; }

    public List<IUI> SceneUIList = new();

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject _warningTextPrefab;

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
            return Instantiate(prefab);
        }
    }

    // BaseUI를 상속하고있는 팝업만 생성
    public T ShowPopUp<T>() where T : BaseUI
    {
        T prefab = Resources.Load<T>($"PopUp/{typeof(T).Name}");
        T instance = Instantiate(prefab, PopUpCanvas.transform);

        PopUpCanvas.ShowUI(instance);
        return instance;
    }

    // 현재 팝업 제거
    public void ClosePopUp()
    {
        PopUpCanvas.CloseUI();
    }

    public void ShowDamageText(Transform spawnPos, long damage)
    {
        if (damageTextPrefab.Equals(null)) return;

        var go = Instantiate(damageTextPrefab, spawnPos.position, Quaternion.identity);
        go.GetComponent<DamageText>()?.InitFloatingDamage($"{damage}");
    }

    public void ShowDamageText(Transform spawnPos, long damage, Color color)
    {
        if (damageTextPrefab.Equals(null)) return;

        var go = Instantiate(damageTextPrefab, spawnPos.position, Quaternion.identity);
        go.GetComponent<DamageText>()?.InitFloatingDamage($"{damage}", color);
    }

    public void ShowWarningText(string text)
    {
        if (_warningTextPrefab.Equals(null)) return;

        var go = Instantiate(_warningTextPrefab);
        go.GetComponent<DamageText>()?.InitWarningMessage($"{text}");
    }
}
