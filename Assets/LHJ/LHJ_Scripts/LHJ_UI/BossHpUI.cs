using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private TMP_Text bossHpText;
    [SerializeField] private BaseBossFSM boss;

    public void Init(BaseBossFSM bossTarget)
    {
        boss = bossTarget;
    }

    private void Update()
    {
        if (boss == null) return;

        float ratio = (float)boss.CurrentHealth / boss.MaxHealth;
        bossHpSlider.value = ratio;
        bossHpText.text = $"{boss.CurrentHealth}/{boss.MaxHealth}";
    }
}
