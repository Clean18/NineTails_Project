using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// �±� ���� ����ü
/// </summary>
public struct PromotionInfo
{
    public int CurrentGrade;       // ���� ����� ���
    public int UpgradeGrade;       // �±� ��� ���   
    public int RequirementLev;     // �±޿� �ʿ��� ���� �䱸ġ
    public int WarmthCost;         // �±޿� �ʿ��� ��ȭ����
}
public class Promotion : MonoBehaviour
{
    public DataTableParser<PromotionInfo> promotionTable;
    void Start()
    {
        // �±� ������ �ٿ�ε� ��ƾ ����
        StartCoroutine(DownloadRoutine());
    }

    //CSV �ٿ�ε�� ���������Ʈ URL
    public const string PromotionTableURL = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&gid=749900094";
    IEnumerator DownloadRoutine()
    {
       UnityWebRequest request = UnityWebRequest.Get(PromotionTableURL);
       yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;
        promotionTable.Parse = words =>
        {
            PromotionInfo info;
            info.CurrentGrade = int.Parse(words[0]);       // ���� ���
            info.UpgradeGrade = int.Parse(words[1]);       // �±��� �� ��� ���
            info.RequirementLev = int.Parse(words[2]);     // �±޿� �ʿ��� ����
            info.WarmthCost = int.Parse(words[3]);         // �±� ���
            return info;
        };
        promotionTable.Load(csv);
    }
}
