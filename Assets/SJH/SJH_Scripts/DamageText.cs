using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
    [SerializeField] private float speed;
    [SerializeField] private float duration;


	public void Init(string dmgText)
	{
		text.text = dmgText;

        StartCoroutine(DamageTextRoutine());
	}


    IEnumerator DamageTextRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            float t = timer / duration;
            text.alpha = 1f - t;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
