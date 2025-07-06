using System.Linq;
using UnityEngine;

public class MonsterHpBar : MonoBehaviour
{
    private Transform fillTransform;
    private float fullWidth;

    void Awake()
    {
        var fillRenderer = GetComponentInChildren<SpriteRenderer>();

        if (fillRenderer == null)
        {
            Debug.Log($"fillRenderer 없음");
            return;
        }

        fillTransform = fillRenderer.transform;
        fullWidth = fillTransform.localScale.x;
    }

    public void SetHealth(float currentHp, float maxHp)
    {
        if (fillTransform == null) return;
        if (maxHp <= 0f) return;

        float hpPercentage = Mathf.Clamp01(currentHp / maxHp);
        float newWidth = fullWidth * hpPercentage;
        fillTransform.localScale = new Vector3(fullWidth * hpPercentage, fillTransform.localScale.y, fillTransform.localScale.z);
    }
}