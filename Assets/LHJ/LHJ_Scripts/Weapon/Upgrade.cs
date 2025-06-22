using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ��ȭ ���� ����ü
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
    [SerializeField] private int currentGrade;      // ���� ��� ���
    [SerializeField] private int currentLevel;      // ���� ��� ��ȭ
    [SerializeField] private int currentAttack;     // ���� ��� ���ݷ� ��ġ
    [SerializeField] private int warmth;            // (Test��) ������ �ִ� ��ȭ
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryEnhance();
        }
    }
    /// <summary>
    /// ��ȭ ���̺��� ���� ��ް� ������ �´� ���� ������ ã�� ��ȭ
    /// ��ȭ�� ����� ��� ��ȭ ���� ó�� �� ���� ���ݷ°� ��ȭ ������ ����
    /// </summary>
    public void TryEnhance()
    {
        // ��ȭ ���̺��� ���� ��ȭ ����ã��
        List<UpgradeInfo> upgradeList = upgradeTable.Values;
        UpgradeInfo nextInfo = default;
        bool found = false;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            UpgradeInfo info = upgradeList[i];

            // ���� ��ް� ���� ��ȭ �ܰ� ������ ��ġ�ϴ���
            if (info.Grade == currentGrade && info.Level == currentLevel + 1)
            {
                nextInfo = info;
                found = true;
                break;
            }
        }

        // ���� ��ȭ ������ �������
        if (!found)
        {
            Debug.Log("�� �̻� ��ȭ�� �� �����ϴ�.");
            return;
        }

        // ��ȭ Ȯ��
        if (warmth < nextInfo.WarmthCost)
        {
            Debug.Log("��ȭ��");
            return;
        }
        else
        {
            // ��ȭ ���� ó��
            warmth -= nextInfo.WarmthCost;
            currentLevel += 1;
            currentAttack = nextInfo.Attack;
            Debug.Log("��ȭ ����! ���� ���: " + currentGrade + " , ��ȭ �ܰ�: " + currentLevel + " , ���ݷ�: " + currentAttack);
        }

        // ���� ��ȭ �ܰ谡 �ִ�� ���޵Ǿ�����
        if (currentLevel >= 5)
        {
            Debug.Log("�ִ� ��ȭ ����");
        }
    }
}
