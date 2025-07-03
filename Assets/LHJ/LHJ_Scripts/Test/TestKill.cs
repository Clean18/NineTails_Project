using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestKill : MonoBehaviour
{
    void Update()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.K))
        {
            string stageId = SceneManager.GetActiveScene().name;    // 현재 씬이름을 스테이지Id로 사용
            AchievementManager.Instance?.KillCount(stageId);        // 해당 씬 킬 업적 체크
            Debug.Log($"[테스트] K 입력으로 {stageId}에서 킬");
        }
    }
}
