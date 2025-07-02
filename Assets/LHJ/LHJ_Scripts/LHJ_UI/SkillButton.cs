using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IUI
{
    /// <summary>
    /// 배열형식으로 관리
    /// skillButtons: 각 스킬 버튼
    /// coolTimeImages: 쿨타임을 표현할 이미지
    /// coolTimes: 각 스킬의 쿨타임 설정 값
    /// triggerKeys: 키 입력으로 스킬 사용
    /// </summary>
    [SerializeField] private Button[] skillButtons;
    [SerializeField] private Image[] coolTimeImages;
    [SerializeField] private float[] coolTimes;
    [SerializeField] private KeyCode[] triggerKeys;

    // 현재 각 스킬별 쿨타임
    [SerializeField] private float[] currentCooltimes;

    public void UIInit()
    {
        Debug.Log("스킬 단축키 UI 초기화");
        var mappingSkills = PlayerController.Instance.GetMappingSkills();

        triggerKeys = new KeyCode[4]
        {
            KeyCode.Mouse0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
        };

        // 현재 스킬들 쿨타임 추가
        coolTimes = new float[4];
        for (int i = 0; i < coolTimes.Length; i++)
        {
            if (mappingSkills.TryGetValue(triggerKeys[i], out var skill) && skill != null && skill.SkillData != null) coolTimes[i] = skill.SkillData.CoolTime;
            else coolTimes[i] = 1f;
        }

        currentCooltimes = new float[skillButtons.Length];

        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            skillButtons[i].onClick.AddListener(() => UseSkill(index));     // 버튼 클릭시 해당 스킬 발동
            coolTimeImages[i].fillAmount = 0;   // 해당 스킬 쿨타임 이미지 초기화
        }
    }

    private void Start()
    {
        UIManager.Instance.SceneUIList.Add(this);
    }

    private void Update()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            // 키보드 키 입력(쿨타임중이 아닐때)
            if (Input.GetKeyDown(triggerKeys[i]) && currentCooltimes[i] <= 0)
            {
                UseSkill(i);
            }

            // 쿨타임 감소 처리
            if (currentCooltimes[i] > 0)
            {
                currentCooltimes[i] -= Time.deltaTime;
                coolTimeImages[i].fillAmount = currentCooltimes[i] / coolTimes[i];  // 쿨타임이미지 fillamount 갱신

                // 쿨타임이 끝났을때 스킬 활성화
                if (currentCooltimes[i] <= 0)
                {
                    skillButtons[i].interactable = true;   // 스킬버튼 클릭 활성화
                    coolTimeImages[i].fillAmount = 0;      
                }
            }
        }
    }

    // 스킬 사용 처리 
    private void UseSkill(int index)
    {
        if (currentCooltimes[index] > 0) return;

        Debug.Log($"스킬 {index} 사용");
        PlayerController.Instance.UseSkill(index);
        currentCooltimes[index] = coolTimes[index]; // 쿨타임
        skillButtons[index].interactable = false;   // 스킬 버튼 클릭 비활성화
    }
}
