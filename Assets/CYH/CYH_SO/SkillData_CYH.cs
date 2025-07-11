using UnityEngine;
using UnityEngine.Audio;

public abstract class SkillData_CYH : ScriptableObject
{
    [field: SerializeField] public string SkillName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public float CoolTime { get; private set; }
    [field: SerializeField] public Sprite SkillSprite { get; private set; }

    [field: SerializeField] public AudioClip SkillAudioClip { get; private set; }
}
