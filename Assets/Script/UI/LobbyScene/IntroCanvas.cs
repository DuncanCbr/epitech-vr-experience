using System.Collections;
using UnityEngine;
public class IntroCanvas : MonoBehaviour
{

    [SerializeField] private float displayDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private CanvasGroup canvasGroupImage;
    [SerializeField] private CanvasGroup canvasGroupBlackScreen;
    [SerializeField] private GameObject canvasBlackScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayTutorialScreen());
    }

    private IEnumerator PlayTutorialScreen()
    {
        yield return StartCoroutine(Fade(0f, 1f, canvasGroupImage));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(Fade(1f, 0f, canvasGroupImage));
        yield return StartCoroutine(Fade(1f, 0f, canvasGroupBlackScreen));
        
        SoundLobby.Instance.StartAmbianceLobbySound();
        
        gameObject.SetActive(false);
        canvasBlackScreen.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, CanvasGroup canvasGroup)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

}
