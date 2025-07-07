using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 미션을 실행하고 진행을 관리하는 매니저
/// </summary>
public class MissionManager : Singleton<MissionManager>
{
    private MissionInfo currentMission;     // 현재 진행 미션
    private float timer;                    // 남은 시간
    public int killCount;                   // 킬 횟수
    private bool isRunning;                 // 미션 실행 여부
    public HashSet<string> MissionIds = new();  // 미션 중복 방지

    public bool IsCooldownActive { get; private set; }      // 외부에서 쿨타임 여부 확인용
    public float CooldownSeconds { get; private set; }        // 남은 쿨타임 초
    // 미션을 실행하는 함수
    public void StartMission(string sceneName)
    {
        // 미션 정보 가져오기
        if (!DataManager.Instance.MissionTable.TryGetValue(sceneName, out currentMission))
        {
            Debug.Log("해당 스테이지 미션 정보 없음");
            return;
        }
        // 클리어 미션이면 재실행하지않음
        //if (MissionIds.Contains(currentMission.Id))
        //{
        //    return;
        //}
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
            // 스테이지 클리어 업적 체크
            AchievementManager.Instance.CheckStageClear(SceneManager.GetActiveScene().name);
            // 이미 클리어한 돌파미션은 보상 지급 X
            MissionIds.Add(currentMission.Id);  // 미션 클리어 
            Reward(currentMission); // 미션 보상
            if (currentMission.Id == "M9999") // M1미션일때
            {
                Time.timeScale = 0;
                Debug.Log("닉네임 팝업창 생성");
                UIManager.Instance.ShowPopUp<NameInputPopUp>(); // 닉네임 팝업창 생성
            }
            else
            {
                // 그 외 미션은 일반 클리어 팝업
                Debug.Log("돌파미션 클리어 팝업창 생성");
                UIManager.Instance.ShowPopUp<CompletePopUp>();  // 미션 성공 팝업창 생성
            }
        }
        else
        {
            Debug.Log("[MissionManager] 미션 실패 (시간 초과)");
            StartCoroutine(CooldownRoutine());               // 쿨타임 시작
            UIManager.Instance.ShowPopUp<FailedPopUp>();     // 실패 팝업 추가
        }
    }
    // 미션 실패 쿨타임
    IEnumerator CooldownRoutine()
    {
        IsCooldownActive = true;    
        CooldownSeconds = 600f;     // 쿨타임 시간 설정

        while (CooldownSeconds > 0)
        {
            yield return new WaitForSeconds(1f);
            CooldownSeconds--;      // 남은 쿨타임 감소
        }

        IsCooldownActive = false;
    }
    public void AddKill()
    {
        // 미션이 실행 중이 아닐때 무시
        if (!isRunning) return;

        killCount++;  // 킬 수 증가
        Debug.Log($"[MissionManager] 현재 킬 수: {killCount}/{currentMission.Count}");

        // 시간 안에 목표 킬 수를 달성하면 미션 성공
        if (killCount >= currentMission.Count && timer > 0f)
        {
            isRunning = false;
        }
    }
    public void DeathFailMission()
    {
        isRunning = false;
        Debug.Log("[MissionManager] 미션 실패 (플레이어 사망)");
        StartCoroutine(CooldownRoutine());
    }
    // 미션 진행중 확인 여부
    public bool IsRunning()
    {
        return isRunning;
    }

    // 미션 시간 반환
    public float GetRemainingTime()
    {
        return timer;
    }

    // 다음 씬 반환
    public string GetNextScene()
    {
        return currentMission.NextScene;
    }
    
    // 미션 보상
    private void Reward(MissionInfo mission)
    {
        // 보상 추가
        if (!IsCleared(mission.Id))
        {
            Debug.Log($"[보상] 온정 +{mission.WarmthReward}, 영기 +{mission.SpritReward}, 스킬 포인트 +{mission.SkillPoint}");
            PlayerController.Instance.AddCost(CostType.Warmth, mission.WarmthReward);
            PlayerController.Instance.AddCost(CostType.SpiritEnergy, mission.SpritReward);
            PlayerController.Instance.AddCost(CostType.Soul, mission.SkillPoint);
        }
        else
        {
            Debug.Log($"이미 획득한 미션입니다. : {mission.Id}");
        }
    }
    public bool IsCleared(string missionId)
    {
        return MissionIds.Contains(missionId);
    }
}
