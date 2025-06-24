using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField] private float DamagePercent = 0.1f; // 최대 체력의 10%
    [SerializeField]  string TargetTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TargetTag))
        {
            var Player = collision.GetComponent<Game.Data.PlayerData>();

            if (Player != null)
            {
                Player.TakeDamageByPercent(DamagePercent);
                Debug.Log($"낙석 충돌: {TargetTag}에게 {DamagePercent * 100}% 데미지");
            }
            // 돌 제거
            Destroy(gameObject);
        }

        
        
    }
}
