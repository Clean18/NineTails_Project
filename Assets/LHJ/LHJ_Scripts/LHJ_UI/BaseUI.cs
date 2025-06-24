using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<string, Component> componentDic;

    private void Awake()
    {
        //BaseUi를 기준으로 모든 자식 게임오브젝트의 컴포넌트를 가져오기(비활성화된 오브젝트 포함)
        RectTransform[] transforms = GetComponentsInChildren<RectTransform>(true);
        gameObjectDic = new Dictionary<string, GameObject>();
        foreach(RectTransform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<string, Component>();
        foreach(Component child in components)
        {
            componentDic.TryAdd($"{child.gameObject.name}_{child.GetType().Name}", child);
        }
    }
    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

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
        return gameObject.GetOrAddComponent<PointerHandler>();
    }
}
