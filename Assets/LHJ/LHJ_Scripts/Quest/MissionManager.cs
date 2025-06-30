using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 미션을 실행하고 진행을 관리하는 매니저
/// </summary>
public class MissionManager : Singleton<AchievementManager>
{
    private MissionInfo currentMission;     // 현재 진행 미션
    private float timer;                    // 남은 시간
    private int killCount;                  // 킬 횟수
    private bool isRunning;                 // 미션 실행 여부


    // 미션을 실행하는 함수
    public void StartMission()
    {
        string stage = SceneManager.GetActiveScene().name;

        // 미션 정보 가져오기
        if (!DataManager.Instance.MissionTable.TryGetValue(stage, out currentMission))
        {
            Debug.Log("해당 스테이지 미션 정보 없음");
            return;
        }
        // 초기값 설정
        timer = currentMission.TimeLimit;
        killCount = 0;
        isRunning = true;

        StartCoroutine(MissionTimerRoutine());
        Debug.Log($"[MissionManager] 미션 시작됨 - {timer}초 안에 {currentMission.Count}킬");
    }

    // 미션 제한시간 루틴
    IEnumerator MissionTimerRoutine()
    {
        // 제한시간이 남아 있고, 킬 수가 목표치보다 적을 동안 반복 실행
        while (timer > 0f && killCount < currentMission.Count)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isRunning = false;

        // 킬수가 미션 수치량 보다 높거나 같을때
        if (killCount >= currentMission.Count)
        {
            Debug.Log("[MissionManager] 미션 성공 (시간 내 클리어)");
            SceneManager.LoadScene(currentMission.NextScene); // 다음씬으로 이동
        }
        else
        {
            Debug.Log("[MissionManager] 미션 실패 (시간 초과)");
        }
    }
}
