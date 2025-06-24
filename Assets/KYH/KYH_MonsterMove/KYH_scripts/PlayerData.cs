using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class PlayerData : MonoBehaviour
    {
        [Header("Health Settings")]
        public float MaxHealth = 100f;
        private float currentHealth;

        private void Start()
        {
            currentHealth = MaxHealth;
        }

        /// <summary>
        /// 퍼센트 단위로 데미지를 받음 (0.1f = 10%)
        /// </summary>
        public void TakeDamageByPercent(float percent)
        {
            float damage = MaxHealth * percent;
            currentHealth -= damage;
            Debug.Log($"플레이어가 {percent * 100f}% 데미지를 입었습니다! 현재 체력: {currentHealth}");

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("플레이어가 사망했습니다!");
            gameObject.SetActive(false); // 테스트용: 사망 시 비활성화
        }
    }
}
