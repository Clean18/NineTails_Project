using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// UIManager에서 생성한 플로팅 대미지 프리팹을 관리하는 스크립트
/// </summary>
public class DamageText : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private float offset;


	public void Init(string dmgText)
	{
		text.text = dmgText;
        transform.position += Vector3.up * offset;
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
