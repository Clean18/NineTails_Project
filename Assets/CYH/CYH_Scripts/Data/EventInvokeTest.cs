using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 저장 요청 이벤트(GameEvents.RequestSave)를 트리거하는 테스트 클래스입니다.
/// </summary>

public class EventInvokeTest : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            KilledMonster();
        }
    }

    public void KilledMonster()
    {
        Debug.Log("플레이어가 몬스터를 잡았음");
        GameEvents.RequestSave();
    }
}
