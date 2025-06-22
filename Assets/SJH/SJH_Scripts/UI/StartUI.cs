using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : SceneUI
{
	void Start()
	{
		UIManager.Instance.StartUI = this;
	}
}
