using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager에서 생성한 플로팅 대미지 프리팹을 관리하는 스크립트
/// </summary>
public class DamageText : MonoBehaviour
{
    // 대미지용
	[SerializeField] private TMP_Text _text;
    // 경고문용
	[SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _speed;
    [SerializeField] private float _duration;
    [SerializeField] private float _offset;

    #region 플로팅 대미지 텍스트용
    public void InitFloatingDamage(string dmgText)
	{
		_text.text = dmgText;
        transform.position += Vector3.up * _offset;
        StartCoroutine(DamageTextRoutine());
	}

    public void InitFloatingDamage(string dmgText, Color color)
    {
        _text.text = dmgText;
        _text.color = color;
        transform.position += Vector3.up * _offset;
        StartCoroutine(DamageTextRoutine());
    }

    IEnumerator DamageTextRoutine()
    {
        float timer = 0f;

        while (timer < _duration)
        {
            transform.position += Vector3.up * _speed * Time.deltaTime;
            float t = timer / _duration;
            _text.alpha = 1f - t;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    #endregion

    #region 경고문용
    public void InitWarningMessage(string messageText)
    {
        _text.text = messageText;
        StartCoroutine(MessageTextRoutine());
    }

    IEnumerator MessageTextRoutine()
    {
        float timer = 0f;
        while (timer < _duration)
        {
            _rectTransform.localPosition += Vector3.up * _speed * Time.deltaTime;
            float t = timer / _duration;
            _canvasGroup.alpha = 1f - t;

            timer += Time.deltaTime;
            yield return null;
        }
        _canvasGroup.alpha = 0;
        Destroy(gameObject);
    }
    #endregion
}
