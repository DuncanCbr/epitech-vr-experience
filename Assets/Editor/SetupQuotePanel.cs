using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Quotes;
using Quotes.UI;

/// <summary>
/// Editor utility to create the Quote Panel GameObject hierarchy with all
/// required UI elements and components pre-wired. Accessible via menu:
/// EPITECH VR > Setup Quote Panel
/// </summary>
public class SetupQuotePanel : Editor
{
    [MenuItem("EPITECH VR/Setup Quote Panel")]
    public static void CreateQuotePanel()
    {
        // Root GameObject
        GameObject root = new GameObject("QuotePanel");
        Undo.RegisterCreatedObjectUndo(root, "Create Quote Panel");

        // -- Canvas (World Space) --
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1f;

        root.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = root.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1600, 900);
        canvasRect.localScale = Vector3.one * 0.0015f; // ~2.4m x 1.35m in world space


        // -- Background --
        GameObject bgObj = CreateUIElement("Background", root, Vector2.zero, new Vector2(1600, 900));
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.10f, 0.10f, 0.18f, 0.95f);

        // -- Border (subtle frame to resemble a chalkboard) --
        Outline border = bgObj.AddComponent<Outline>();
        border.effectColor = new Color(0.3f, 0.3f, 0.4f, 0.6f);
        border.effectDistance = new Vector2(4, -4);

        // -- Quote Text (centered, large, wrapping) --
        Text quoteText = CreateText(
            "QuoteText", bgObj,
            new Vector2(0, 50),
            new Vector2(1500, 700),
            "\"Loading...\"",
            75, TextAnchor.MiddleCenter,
            HorizontalWrapMode.Wrap
        );
        quoteText.lineSpacing = 1.2f;

        // -- Author Text (bottom-right, italic) --
        Text authorText = CreateText(
            "AuthorText", bgObj,
            new Vector2(0, -380),
            new Vector2(1500, 80),
            "- ...",
            55, TextAnchor.MiddleRight,
            HorizontalWrapMode.Overflow
        );
        authorText.fontStyle = FontStyle.Italic;
        authorText.color = new Color(0.7f, 0.7f, 0.8f, 1f);

        // -- Wire up QuotePanelUI component --
        QuotePanelUI panelUI = root.AddComponent<QuotePanelUI>();

        SerializedObject serializedPanel = new SerializedObject(panelUI);
        serializedPanel.FindProperty("txtQuote").objectReferenceValue = quoteText;
        serializedPanel.FindProperty("txtAuthor").objectReferenceValue = authorText;
        serializedPanel.ApplyModifiedProperties();

        // -- Wire up QuoteManager component --
        QuoteManager manager = root.AddComponent<QuoteManager>();
        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("panelUI").objectReferenceValue = panelUI;
        serializedManager.ApplyModifiedProperties();

        // Select the created object
        Selection.activeGameObject = root;
        Debug.Log("[SetupQuotePanel] Quote panel created successfully. " +
            "Make sure to create a .env file with API_NINJAS_KEY at the project root.");
    }

    private static GameObject CreateUIElement(string name, GameObject parent, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
        return obj;
    }

    private static Text CreateText(
        string name, GameObject parent,
        Vector2 pos, Vector2 size,
        string content, int fontSize,
        TextAnchor alignment,
        HorizontalWrapMode wrapMode)
    {
        GameObject obj = CreateUIElement(name, parent, pos, size);
        Text text = obj.AddComponent<Text>();
        text.text = content;

        // Robust font fallback chain
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (f == null) f = Font.CreateDynamicFontFromOSFont("Arial", 16);
        text.font = f;

        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;

        text.horizontalOverflow = wrapMode;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }
}
