using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{

    // 캐릭터 스텟
    public PlayerData PlayerData;

    // 재화
    // 온정
    // 스킬 해금 포인트
    // 영기 (장비강화)

    public PlayerModel()
    {
        // 생성자에서 캐릭터스탯, 재화, 스킬, 장비 등 인스턴스화
        PlayerData = new PlayerData();
    }
}
