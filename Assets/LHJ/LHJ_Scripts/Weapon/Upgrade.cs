using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ��ȭ�� �ʿ��� ������
/// </summary>
[System.Serializable]
public struct UpgradeInfo
{
    public int Grade;       // ����� ���
    public int Level;       // ��ȭ �ܰ�    
    public int Attack;      // ���ݷ� ��ġ
    public int WarmthCost;  // ��ȭ�� �ʿ��� ��ȭ����
}
[System.Serializable]
public class UpgradeTable : DataTableParser<UpgradeInfo>
{
    public UpgradeTable(Func<string[], UpgradeInfo> Parse) : base(Parse)
    {
    }
}

public class Upgrade : MonoBehaviour
{
    public UpgradeTable upgradeTable;
    void Start()
    {
        // ��ȭ ������ �ٿ�ε� ��ƾ ����
        StartCoroutine(DownloadRoutine());
    }

    //CSV �ٿ�ε�� ���������Ʈ URL
    public const string UpgradeTableURL = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&&gid=0";
    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(UpgradeTableURL);
        yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;
        upgradeTable.Parse = words =>
        {
            UpgradeInfo info;
            info.Grade = int.Parse(words[0]);       // ���
            info.Level = int.Parse(words[1]);       // ��ȭ �ܰ�
            info.Attack = int.Parse(words[2]);      // ���ݷ�  
            info.WarmthCost = int.Parse(words[3]);  // ��ȭ ���
            return info;
        };
        upgradeTable.Load(csv);
    }
    // TODO: ��ȭ���� ����
}
