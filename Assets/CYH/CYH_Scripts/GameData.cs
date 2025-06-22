using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save/Load �Ǿ�� �� ����/ScriptableObject�� ��� Ŭ�����Դϴ�.
/// ��� �� Inspectorâ �������� ���� Header�� �ʼ��� �ۼ����ּ��� ex.[Header("Player")]
/// </summary>
[System.Serializable]
public partial class GameData
{
    [Header("������ ���� �ð�")]
    [SerializeField] private string _savedTimeString;

    // DateTime ����ȭ/������ȭ ������Ƽ
    public DateTime SavedTime
    {
        get => DateTime.Parse(_savedTimeString);         // string > DateTime
        set => _savedTimeString = value.ToString("o");   // DateTime > string
    }

    [Header("�׽�Ʈ�� �⺻ ��")]
    public int Level_T = 1;
    public float Exp_T = 10;
    public int Gold_T = 100;
}
