using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class LoadingFade : MonoBehaviour
{
    [SerializeField] private Image _screen;
    [SerializeField] private float _fadeOutDuration;
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;

    public void FadeStart() => StartCoroutine(FadeOut());

    IEnumerator FadeOut()
    {
        // 맨 처음 시작컬러
        Debug.Log("페이드아웃 시작");
        _screen.color = _startColor;
        yield return null;

        float timer = 0;

        while (timer < _fadeOutDuration)
        {
            float t = timer / _fadeOutDuration;

            Color lerp = Color.Lerp(_startColor, _endColor, t);
            _screen.color = lerp;

            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("페이드아웃 종료");
        _screen.color = _endColor;
    }
}
