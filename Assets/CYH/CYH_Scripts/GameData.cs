using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save/Load 되어야 할 변수/ScriptableObject를 담는 클래스입니다.
/// 사용 시 Inspector창 가독성을 위해 Header를 필수로 작성해주세요 ex.[Header("Player")]
/// </summary>
[System.Serializable]
public partial class GameData
{
    [Header("데이터 저장 시간")]
    [SerializeField] private string _savedTimeString;

    // DateTime 직렬화/역직렬화 프로퍼티
    public DateTime SavedTime
    {
        get => DateTime.Parse(_savedTimeString);         // string > DateTime
        set => _savedTimeString = value.ToString("o");   // DateTime > string
    }

    [Header("테스트용 기본 값")]
    public int Level_T = 1;
    public float Exp_T = 10;
    public int Gold_T = 100;
}
