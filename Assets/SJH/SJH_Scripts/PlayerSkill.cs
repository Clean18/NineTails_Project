using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkill
{
    public Dictionary<KeyCode, ISkill> SkillMapping;
    private SkillController _controller;
    public SkillLogic_0_HitBox DefaultAttack;
    public SkillLogic_1 Skill1;
    public SkillLogic_2 Skill2;

    public void InitSkill()
    {
        _controller = PlayerController.Instance.SkillController;

        // TODO : 단축키 완성되면 연결
        if (_controller.SkillList[0] is SkillLogic_0_HitBox skill) DefaultAttack = skill;
        if (_controller.SkillList[1] is SkillLogic_1 skill1) Skill1 = skill1;
        if (_controller.SkillList[2] is SkillLogic_2 skill2) Skill2 = skill2;

        SkillMapping = new()
        {
            [KeyCode.Alpha1] = Skill1,
            [KeyCode.Alpha2] = Skill2,
            [KeyCode.Alpha3] = null,
        };
        Debug.Log("플레이어 스킬 초기화 완료");
    }

    public ISkill GetSkill(KeyCode keyCode) => SkillMapping.TryGetValue(keyCode, out var value) ? value : null;
}