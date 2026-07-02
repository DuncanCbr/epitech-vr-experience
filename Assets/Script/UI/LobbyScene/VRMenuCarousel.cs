using UnityEngine;

public class VRMenuCarousel : MonoBehaviour
{
    [Header("Menus")]
    public Transform[] menus;

    [Header("Slots")]
    public Transform leftSlot;
    public Transform centerSlot;
    public Transform rightSlot;

    [Header("Animation")]
    public float moveSpeed = 10f;
    public float sideScale = 0.75f;
    public float centerScale = 1f;

    [Header("Opacity")]
    public float sideAlpha = 0.5f;
    public float centerAlpha = 1f;

    [Header("Grab / Drag")]
    public float swipeThreshold = 0.25f;

    private int currentIndex = 0;
    private bool hasSwiped = false;
    private Vector3 grabStartPosition;

    void Start()
    {
        UpdateSiblingOrder();
    }

    void Update()
    {
        UpdateMenuPositions();
    }

    void UpdateMenuPositions()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            int offset = GetCircularOffset(i, currentIndex, menus.Length);

            Transform targetSlot = null;
            float targetScale = sideScale;
            float targetAlpha = sideAlpha;
            bool visible = true;

            if (offset == -1)
            {
                targetSlot = leftSlot;
            }
            else if (offset == 0)
            {
                targetSlot = centerSlot;
                targetScale = centerScale;
                targetAlpha = centerAlpha;
            }
            else if (offset == 1)
            {
                targetSlot = rightSlot;
            }
            else
            {
                visible = false;
            }

            menus[i].gameObject.SetActive(visible);

            if (!visible)
                continue;

            menus[i].position = Vector3.Lerp(
                menus[i].position,
                targetSlot.position,
                Time.deltaTime * moveSpeed
            );

            menus[i].rotation = Quaternion.Slerp(
                menus[i].rotation,
                targetSlot.rotation,
                Time.deltaTime * moveSpeed
            );

            menus[i].localScale = Vector3.Lerp(
                menus[i].localScale,
                Vector3.one * targetScale,
                Time.deltaTime * moveSpeed
            );

            CanvasGroup canvasGroup = menus[i].GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(
                    canvasGroup.alpha,
                    targetAlpha,
                    Time.deltaTime * moveSpeed
                );
            }
        }
    }

    int GetCircularOffset(int index, int current, int count)
    {
        int offset = index - current;

        if (offset > count / 2)
            offset -= count;

        if (offset < -count / 2)
            offset += count;

        return offset;
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % menus.Length;
        UpdateSiblingOrder();
    }

    public void Previous()
    {
        currentIndex = (currentIndex - 1 + menus.Length) % menus.Length;
        UpdateSiblingOrder();
    }

    void UpdateSiblingOrder()
    {
        menus[currentIndex].SetAsLastSibling();
    }

    public void OnGrabStart(Transform handOrController)
    {
        grabStartPosition = handOrController.position;
        hasSwiped = false;
    }

    public void OnGrabMove(Transform handOrController)
    {
        if (hasSwiped)
            return;

        float deltaX = handOrController.position.x - grabStartPosition.x;

        if (deltaX < -swipeThreshold)
        {
            Next();
            hasSwiped = true;
        }
        else if (deltaX > swipeThreshold)
        {
            Previous();
            hasSwiped = true;
        }
    }

    public void OnGrabEnd()
    {
        hasSwiped = false;
    }
}