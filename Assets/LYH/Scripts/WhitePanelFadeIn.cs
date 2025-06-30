using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WhitePanelFadeIn : MonoBehaviour
{
    public Image whitePanelImage;
    public float fadeDuration = 1.5f;
    public string nextSceneName = "CharSelect"; // 전환할 씬 이름

    public void StartFadeAndLoadScene()
    {
        StartCoroutine(FadeInAndLoadScene());
    }

    private System.Collections.IEnumerator FadeInAndLoadScene()
    {
        whitePanelImage.gameObject.SetActive(true); // 패널 켜기
        float elapsed = 0f;
        Color startColor = whitePanelImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            whitePanelImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        whitePanelImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        // 씬 전환
        SceneManager.LoadScene(nextSceneName);
    }
}