using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FloatingMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Flottement vertical")]
    public float floatHeight = 0.02f;
    public float floatSpeed = 1f;

    [Header("Effet au survol")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 8f;

    [Header("Changement de sprite")]
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;

    [Header("Changement de scène")]
    public string sceneToLoad = "epitech_map";
    public GameObject loadingCanvas;
    public float delayBeforeLoading = 2f;

    [Header("Canvas d'aide")]
    public GameObject helpCanvas;

    private Vector3 startPosition;
    private Vector3 baseScale;
    private bool isHovered = false;
    private bool isLoading = false;

    void Start()
    {
        startPosition = transform.localPosition;
        baseScale = transform.localScale;

        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;

        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);

        if (helpCanvas != null)
            helpCanvas.SetActive(false);
    }

    void Update()
    {
        if (!isHovered)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
        }
        else
        {
            transform.localPosition = startPosition;
        }

        Vector3 targetScale = isHovered ? baseScale * hoverScale : baseScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (targetImage != null && hoverSprite != null)
            targetImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (helpCanvas != null)
            helpCanvas.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (helpCanvas != null)
            helpCanvas.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLoading) return;
        isLoading = true;

        Debug.Log("Carte cliquée, chargement de la scène : " + sceneToLoad);

        StartCoroutine(LoadSceneWithDelay());
    }

    private System.Collections.IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoading);

        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}