using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Effect : MonoBehaviour
{
    void OnEnable() => StartCoroutine(DeadRoutine());

    IEnumerator DeadRoutine()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        yield return null;

        float timer = 0f;
        float duration = 3f;
        Color startColor = sprite.color;

        while (timer < duration)
        {
            float t = timer / duration;

            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            sprite.color = newColor;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject, 2f);
    }
}
