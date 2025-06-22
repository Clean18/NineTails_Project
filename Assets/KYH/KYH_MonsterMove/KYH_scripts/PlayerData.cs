using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public int Gold { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"°ñµå {Gold} È¹µæ.");
    }
}
