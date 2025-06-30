using UnityEngine;
using UnityEngine.UI;

public class WhitePanelFadeImage : MonoBehaviour
{
    public Image whitePanelImage;
    public float fadeDuration = 1.5f;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color startColor = whitePanelImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            whitePanelImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        whitePanelImage.gameObject.SetActive(false);
    }
}
