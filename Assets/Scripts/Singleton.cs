using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				// 사용할 매니저는 하이어라키에 있어야함
				// Resources.Load 를 사용하면 Resources 폴더에서 프리팹 상태로 사용가능
				_instance = FindObjectOfType<T>();
				if (_instance == null)
				{
					var obj = new GameObject(typeof(T).Name);
					_instance = obj.AddComponent<T>();
				}
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
		}
	}
}
