using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save/Load 되어야 할 변수/ScriptableObject를 담는 클래스입니다.
/// 사용 시 Inspector창 가독성을 위해 Header를 필수로 작성해주세요 ex.[Header("Player")]
/// </summary>
[System.Serializable]
public partial class GameData
{
    [Header("데이터 저장 시간")]
    [SerializeField] private string _savedTimeString;

    // DateTime 직렬화/역직렬화 프로퍼티
    public DateTime SavedTime
    {
        get => DateTime.Parse(_savedTimeString);         // string > DateTime
        set => _savedTimeString = value.ToString("o");   // DateTime > string
    }

    [Header("PlayerData")]
    // 플레이어 이름
    public string PlayerName = "구미호";
    // 공격 레벨
    public int AttackLevel = 1;
    // 방어 레벨
    public int DefenseLevel = 1;
    // 체력 레벨
    public int HpLevel = 1;
    // 현재 체력
    public long CurrentHp = 100;
    // 이속 레벨
    public int SpeedLevel = 1;
    // 보호막 체력
    public long ShieldHp = 0;
    // 플레이어 마지막 씬의 인덱스
    public int SceneIndex = 2;

    [Header("PlayerCost")]
    // 영기
    public long SpiritEnergy = 0;
    // 온정
    public long Warmth = 0;
    // 혼백
    public long Soul = 0;
    // 영기 첫 획득
    public bool GetFirstSpiritEnergy = false;
    // 온정 첫 획득
    public bool GetFirstWarmth = false;

    [Header("PlayerSkill")]
    public List<SaveSkillData> PlayerSkillList = new()
    {
        // 기본공격
        new SaveSkillData
        {
            SkillIndex = 0,
            SkillLevel = 0,
            SlotIndex = 0,
        }
    };

    [Header("PlayerEquipment")]
    // 장비 등급
    public string Grade = "N";
    // 장비 레벨
    public int Level = 1;
    // 가하는 피해 레벨
    public int IncreaseDamageLevel = 0;

    [Header("PlayerQuest")]
    // 업적
    public List<SaveAchievementData> PlayerAchivementList = new();
    // 미션
    public List<SaveMissionData> PlayerMissionList = new();
    // 돌파미션 리트라이 쿨타임
    public float MissionCooldown = 0;
}
