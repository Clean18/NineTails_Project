using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [SerializeField] private int MaxHp;
    [SerializeField] private int CurrentHp;
    [SerializeField] private int RewardGold;

    public void Init(int hp, int gold)
    {
        MaxHp = hp;
        CurrentHp = hp;
        RewardGold = gold;
    }

    public void TakeDamage(int amount)
    {
        CurrentHp -= amount;
        if( CurrentHp < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Game.Data.PlayerData.Instance.AddGold(RewardGold);

        Destroy(gameObject);
    }
}
