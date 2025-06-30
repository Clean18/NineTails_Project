using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill
{
    public Dictionary<KeyCode, ISkill> SkillMapping;
    public SkillLogic_0_HitBox DefaultAttack;

    public void InitSkill()
    {
        DefaultAttack = GameManager.Instance.PlayerController.GetComponent<SkillLogic_0_HitBox>();
        SkillMapping = new()
        {
            [KeyCode.Alpha1] = GameManager.Instance.PlayerController.GetComponent<SkillLogic_1>(),
            //추가(CYH)
            [KeyCode.Alpha2] = GameManager.Instance.PlayerController.GetComponent<SkillLogic_2>(),
            [KeyCode.Alpha3] = null,
        };

        DefaultAttack.PlayerController = GameManager.Instance.PlayerController;
        SkillMapping[KeyCode.Alpha1].PlayerController = GameManager.Instance.PlayerController;
        // 추가(CYH)
        SkillMapping[KeyCode.Alpha2].PlayerController = GameManager.Instance.PlayerController;
    }

    public ISkill GetSkill(KeyCode keyCode) => SkillMapping.TryGetValue(keyCode, out var value) ? value : null;
}