using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkillData", menuName = "Skills/ActiveSkillData")]
public class ActiveSkillData : SkillData_CYH
{
    [field: SerializeField] public float Range { get; private set; }
}