using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ActiveSkillData")]
public class ActiveSkillData : ScriptableObject
{
    [field: SerializeField] public string SkillName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public float CoolTime { get; private set; }
    [field: SerializeField] public bool IsCooldown { get; private set; } = false;
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public Sprite SkillSprite { get; private set; }
    [field: SerializeField] public Animator SkillAnimation { get; private set; }

    public float GetDamage(PlayerData player)
    {
        // 플레이어 공격력에 데미지를 곱해서 반환
        return Damage * player.Attack;
    }
}

