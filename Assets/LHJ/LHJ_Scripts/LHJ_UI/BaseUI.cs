using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;   // 오브젝트 이름으로 탐색
    private Dictionary<string, Component> componentDic;     // 컴포넌트로 탐색

    private void Awake()
    {
        // 오브젝트에는 트랜스폼이 존재하기때문에 트랜스폼 기준으로 탐색
        RectTransform[] transforms = GetComponentsInChildren<RectTransform>(true);
        gameObjectDic = new Dictionary<string, GameObject>();
        foreach(RectTransform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        // 컴포넌트를 통해 탐색
        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<string, Component>();
        foreach(Component child in components)
        {
            componentDic.TryAdd($"{child.gameObject.name}_{child.GetType().Name}", child);
        }
    }

    // 오브젝트 이름을 반환
    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);     // 게임오브젝트 이름 반환 없으면 null
        return gameObject;
    }

    // 이름 및 컴포넌트로 찾는 구조
    public T GetUI<T>(in string name) where T : Component
    {
        componentDic.TryGetValue($"{name}_{typeof(T).Name}", out Component component);
        if (component != null)
            return component as T;

        GameObject gameObject = GetUI(name);
        if (gameObject == null)
            return null;

        component = gameObject.GetComponent<T>();
        if (component == null)
            return null;

        componentDic.TryAdd($"{name}_{typeof(T).Name}", component);
        return component as T;
    }

    public PointerHandler GetEvent(in string name)
    {
        GameObject gameObject = GetUI(name);
        if (gameObject == null) return null;
        return gameObject.GetOrAddComponent<PointerHandler>();
    }
}
