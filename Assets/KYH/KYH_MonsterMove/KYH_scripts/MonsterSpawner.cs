using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MonsterSpawner : MonoBehaviour
{
    /// <summary>
    /// 몬스터를 일정 간격으로 자동 스폰하는 클래스
    /// - 카메라 시야 밖 랜덤 위치에서 스폰
    /// - 최대 몬스터 수 제한 기능 포함
    /// - 스테이지 레벨에 따라 HP, 골드 난이도 자동 조절
    /// </summary>
    [Header("spawn setting")]
    [SerializeField] private GameObject MonsterPrefabs;  // 스폰할 몬스터 프리팹
    [SerializeField] private float SpawnInterval = 2f;   // 몬스터 생성 간격(초)
    [SerializeField] private int MaxMonsterCount = 10;   // 한번에 존재 가능한 최대 몬스터 수
    [SerializeField] private float SpawnDistance = 2f;   // 카메라 시야 밖으로 얼마나 먼거리에서 스폰할지 거리

    [Header("stage info")]
    [SerializeField] private int StageLevel = 1;        // 현재 스테이지 레벨 ( 난이도 조절용 )
    [SerializeField] private AnimationCurve HpCurve;    // 스테이지별 몬스터 HP 조절용 커브
    [SerializeField] private AnimationCurve GoldCurve;  // 스테이지별 몬스터 처치 보상 골드 조절용 커브
    // 커브는 현재 기능 점검예정 사용이 어려우면 간단한 수식화 할듯.

    private float Timer;  // 내부 시간 측정용 타이머

    private void Update()
    {
        Timer += Time.deltaTime; // 타이머 초기화

        if (Timer >= SpawnInterval)
        {
            Timer = 0f;

            // 현재 씬 내에 존재하는 몬스터 수를 태그로 검색 (성능이슈 있을 수 있음)
            int currentMonsterCount = GameObject.FindGameObjectsWithTag("Monster").Length;

            // 최대 몬스터 수 미만일 경우에만 새 몬스터 생성
            if (currentMonsterCount < MaxMonsterCount)
            {
                SpawnMonster();
            }
        }
    }

    /// <summary>
    /// 카메라 시야 밖 랜덤 위치를 계산해 몬스터를 생성하는 함수
    /// </summary>
    private void SpawnMonster()
    {
        // 시야 밖 랜덤 위치 얻기
        Vector3 SpawnPosition = GetRandomPositionOutSideView();
        // 인스턴스로 몬스터 생성
        GameObject Monster = Instantiate(MonsterPrefabs, SpawnPosition, Quaternion.identity);
        // 생성된 몬스터의 스텟 컴포넌트 불러오기
        var MonsterStat = Monster.GetComponent<MonsterStat>();

        // 현재 스테이지 레벨에 따른 HP 설정과 드랍골드 커브로 관리
        MonsterStat.Init(
            (int)HpCurve.Evaluate(StageLevel),
            (int)GoldCurve.Evaluate(StageLevel)
            );
    }

    /// <summary>
    /// 카메라 시야 밖 네 방향 중 랜덤으로 위치를 반환하는 함수
    /// - 시야 밖 거리만큼 떨어진 위치를 기준으로 랜덤 좌표 생성
    /// </summary>
    /// <returns>랜덤으로 선택된 시야 밖 월드 좌표</returns>
    private Vector3 GetRandomPositionOutSideView()
    {
        Camera Cam = Camera.main;

        // 카메라의 세로 절반 길이 계산 
        float CamHeight = 2f * Cam.orthographicSize;

        // 카메라의 가로 절반 길이 계산
        float CamWidth = CamHeight * Cam.aspect;

        // 카메라의 현재 위치
        Vector3 CamPos = Cam.transform.position;

        // 시야 밖으로 얼마나 멀어질지 거리 지정
        float Extra = SpawnDistance;

        // 0 = 위, 1 = 아래, 2 = 왼쪽, 3 = 오른쪽 중에 랜덤 선택
        int Side = Random.Range(0, 4);
        Vector3 offset = Vector3.zero;

        // 선택된 방향에 따라 랜덤 좌표 생성
        switch (Side)
        {
            case 0: // 윗 방향
                offset = new Vector3(Random.Range(-CamWidth / 2, CamWidth / 2), CamHeight / 2 + Extra, 0);
                break;
            case 1: // 아랫 방향
                offset = new Vector3(Random.Range(-CamWidth / 2, CamWidth / 2), -CamHeight / 2 - Extra, 0);
                break;
            case 2: // 왼쪽 방향
                offset = new Vector3(-CamWidth / 2 - Extra, Random.Range(-CamHeight / 2, CamHeight / 2), 0);
                break;
            case 3: // 오른쪽 방향
                offset = new Vector3(CamWidth / 2 + Extra, Random.Range(-CamHeight / 2, CamHeight / 2), 0);
                break;
        }

        // 카메라 위치 + 오프셋 = 최종 스폰위치 반환
        return CamPos + offset;
    }
}
