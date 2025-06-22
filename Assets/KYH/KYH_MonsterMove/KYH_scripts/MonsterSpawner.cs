using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterSpawner : MonoBehaviour
{
    /// <summary>
    /// ���͸� ���� �������� �ڵ� �����ϴ� Ŭ����
    /// - ī�޶� �þ� �� ���� ��ġ���� ����
    /// - �ִ� ���� �� ���� ��� ����
    /// - �������� ������ ���� HP, ��� ���̵� �ڵ� ����
    /// </summary>
    [Header("spawn setting")]
    [SerializeField] private GameObject MonsterPrefabs;  // ������ ���� ������
    [SerializeField] private float SpawnInterval = 2f;   // ���� ���� ����(��)
    [SerializeField] private int MaxMonsterCount = 10;   // �ѹ��� ���� ������ �ִ� ���� ��
    [SerializeField] private float SpawnDistance = 2f;   // ī�޶� �þ� ������ �󸶳� �հŸ����� �������� �Ÿ�

    [Header("stage info")]
    [SerializeField] private int StageLevel = 1;        // ���� �������� ���� ( ���̵� ������ )
    [SerializeField] private AnimationCurve HpCurve;    // ���������� ���� HP ������ Ŀ��
    [SerializeField] private AnimationCurve GoldCurve;  // ���������� ���� óġ ���� ��� ������ Ŀ��
    // Ŀ��� ���� ��� ���˿��� ����� ������ ������ ����ȭ �ҵ�.

    private float Timer;  // ���� �ð� ������ Ÿ�̸�

    private void Update()
    {
        Timer += Time.deltaTime; // Ÿ�̸� �ʱ�ȭ

        if (Timer >= SpawnInterval)
        {
            Timer = 0f;

            // ���� �� ���� �����ϴ� ���� ���� �±׷� �˻� (�����̽� ���� �� ����)
            int currentMonsterCount = GameObject.FindGameObjectsWithTag("Monster").Length;

            // �ִ� ���� �� �̸��� ��쿡�� �� ���� ����
            if (currentMonsterCount < MaxMonsterCount)
            {
                SpawnMonster();
            }
        }
    }

    /// <summary>
    /// ī�޶� �þ� �� ���� ��ġ�� ����� ���͸� �����ϴ� �Լ�
    /// </summary>
    private void SpawnMonster()
    {
        // �þ� �� ���� ��ġ ���
        Vector3 SpawnPosition = GetRandomPositionOutSideView();
        // �ν��Ͻ��� ���� ����
        GameObject Monster = Instantiate(MonsterPrefabs, SpawnPosition, Quaternion.identity);
        // ������ ������ ���� ������Ʈ �ҷ�����
        var MonsterStat = Monster.GetComponent<MonsterStat>();

        // ���� �������� ������ ���� HP ������ ������ Ŀ��� ����
        MonsterStat.Init(
            (int)HpCurve.Evaluate(StageLevel),
            (int)GoldCurve.Evaluate(StageLevel)
            );
    }

    /// <summary>
    /// ī�޶� �þ� �� �� ���� �� �������� ��ġ�� ��ȯ�ϴ� �Լ�
    /// - �þ� �� �Ÿ���ŭ ������ ��ġ�� �������� ���� ��ǥ ����
    /// </summary>
    /// <returns>�������� ���õ� �þ� �� ���� ��ǥ</returns>
    private Vector3 GetRandomPositionOutSideView()
    {
        Camera Cam = Camera.main;

        // ī�޶��� ���� ���� ���� ��� 
        float CamHeight = 2f * Cam.orthographicSize;

        // ī�޶��� ���� ���� ���� ���
        float CamWidth = CamHeight * Cam.aspect;

        // ī�޶��� ���� ��ġ
        Vector3 CamPos = Cam.transform.position;

        // �þ� ������ �󸶳� �־����� �Ÿ� ����
        float Extra = SpawnDistance;

        // 0 = ��, 1 = �Ʒ�, 2 = ����, 3 = ������ �߿� ���� ����
        int Side = Random.Range(0, 4);
        Vector3 offset = Vector3.zero;

        // ���õ� ���⿡ ���� ���� ��ǥ ����
        switch (Side)
        {
            case 0: // �� ����
                offset = new Vector3(Random.Range(-CamWidth / 2, CamWidth / 2), CamHeight / 2 + Extra, 0);
                break;
            case 1: // �Ʒ� ����
                offset = new Vector3(Random.Range(-CamWidth / 2, CamWidth / 2), -CamHeight / 2 - Extra, 0);
                break;
            case 2: // ���� ����
                offset = new Vector3(-CamWidth / 2 - Extra, Random.Range(-CamHeight / 2, CamHeight / 2), 0);
                break;
            case 3: // ������ ����
                offset = new Vector3(CamWidth / 2 + Extra, Random.Range(-CamHeight / 2, CamHeight / 2), 0);
                break;
        }

        // ī�޶� ��ġ + ������ = ���� ������ġ ��ȯ
        return CamPos + offset;
    }
}
