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
    [SerializeField] private Image[] _disableImages;

    // 현재 각 스킬별 쿨타임
    [SerializeField] private float[] currentCooltimes;

    public static SkillButton Instance { get; private set; }


    public void UIInit()
    {
        Instance = this;

        Debug.Log("스킬 단축키 UI 초기화");
        var mappingSkills = PlayerController.Instance.GetMappingSkill();

        triggerKeys = new KeyCode[3]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
        };
        //currentCooltimes = new float[skillButtons.Length];

        // 스킬 아이콘 변경
        UpdateButtonImage();

        // 현재 스킬들 쿨타임 추가 1 ~ 3스킬
        coolTimes = new float[3];
        for (int i = 0; i < coolTimes.Length; i++)
        {
            if (mappingSkills.TryGetValue(triggerKeys[i], out var skill) && skill != null && skill.SkillData != null)
            {
                coolTimes[i] = skill.SkillData.CoolTime;

                // 쿨타임 중이면 남은 시간으로 재할당해서 다시 쿨타임돌아가게
                if (skill.IsCooldown)
                {
                    currentCooltimes[i] = skill.RemainCooldown;
                    _disableImages[i].fillAmount = currentCooltimes[i] / coolTimes[i];
                    _disableImages[i].gameObject.SetActive(true);
                    continue;
                }
            }
            else
            {
                coolTimes[i] = 1f;
            }

            // 쿨타임이 아닌 경우만 초기화
            currentCooltimes[i] = 0f;
            _disableImages[i].fillAmount = 1;
            _disableImages[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i + 1;
            skillButtons[i].onClick.RemoveAllListeners();
            skillButtons[i].onClick.AddListener(() => // 버튼 클릭시 해당 스킬 발동
            {
                Debug.Log($"{index} 스킬버튼 클릭");
                UseSkill(index);
            });
            //coolTimeImages[i].fillAmount = 1;   // 해당 스킬 쿨타임 이미지 초기화
            //_disableImages[i].fillAmount = 1;   // 해당 스킬 쿨타임 이미지 초기화
            //_disableImages[i].gameObject.SetActive(false);
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
            // 쿨타임 감소 처리
            if (currentCooltimes[i] > 0)
            {
                currentCooltimes[i] -= Time.deltaTime;
                _disableImages[i].fillAmount = currentCooltimes[i] / coolTimes[i];  // 쿨타임이미지 fillamount 갱신

                // 쿨타임이 끝났을때 스킬 활성화
                if (currentCooltimes[i] <= 0)
                {
                    //skillButtons[i].interactable = true;   // 스킬버튼 클릭 활성화
                    _disableImages[i].fillAmount = 1;
                    _disableImages[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // 버튼 클릭시 스킬 사용시 쿨타임
    public void UseSkill(int index)
    {
        int uiIndex = index - 1;
        if (uiIndex < 0 || uiIndex >= currentCooltimes.Length) return;
        if (currentCooltimes[uiIndex] > 0) return;

        // 스킬이 사용됐을 때만 실행
        if (PlayerController.Instance.UseSkill(index))
        {
            currentCooltimes[uiIndex] = coolTimes[uiIndex]; // 쿨타임
            _disableImages[uiIndex].gameObject.SetActive(true);
            //skillButtons[uiIndex].interactable = false;   // 스킬 버튼 클릭 비활성화
        }
    }

    // 플레이어가 1 ~ 3 입력, AI가 스킬 사용시 쿨타임
    public void UpdateCooldown(int slotIndex)
    {
        if (Instance == null) return;
        int uiIndex = slotIndex - 1;
        if (uiIndex < 0 || uiIndex >= currentCooltimes.Length) return;
        if (currentCooltimes[uiIndex] > 0) return;

        currentCooltimes[uiIndex] = coolTimes[uiIndex]; // 쿨타임
        _disableImages[uiIndex].gameObject.SetActive(true);
        //skillButtons[uiIndex].interactable = false;   // 스킬 버튼 클릭 비활성화
    }

    public void UpdateButtonImage()
    {
        var mappingSkills = PlayerController.Instance.GetMappingSkill();

        for (int i = 0; i < coolTimeImages.Length; i++)
        {
            if (mappingSkills.TryGetValue(triggerKeys[i], out ISkill skill) && skill != null)
            {
                coolTimeImages[i].sprite = skill.SkillData.SkillSprite;
                coolTimeImages[i].enabled = true;
                _disableImages[i].sprite = skill.SkillData.SkillSprite;
                _disableImages[i].enabled = true;
            }
            else
            {
                coolTimeImages[i].enabled = false;
                _disableImages[i].enabled = false;
            }
        }
    }
}
