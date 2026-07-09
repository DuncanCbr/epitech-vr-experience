using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Weather;
using Weather.UI;

/// <summary>
/// Editor utility to create the Weather Panel GameObject hierarchy with all
/// required UI elements and components pre-wired. Accessible via menu:
/// EPITECH VR > Setup Weather Panel
/// </summary>
public class SetupWeatherPanel : Editor
{
    [MenuItem("EPITECH VR/Setup Weather Panel")]
    public static void CreateWeatherPanel()
    {
        // Root GameObject
        GameObject root = new GameObject("WeatherPanel");
        Undo.RegisterCreatedObjectUndo(root, "Create Weather Panel");

        // -- Canvas (World Space) --
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1f; // IMPORTANT: Must be 1 when using large font sizes in WorldSpace, otherwise texture overflows!

        root.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = root.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1080, 1920);
        canvasRect.localScale = Vector3.one * 0.0015f; // ~1.62m x 2.88m in physical world space

        // -- Background --
        GameObject bgObj = CreateUIElement("Background", root, Vector2.zero, new Vector2(1080, 1920));
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.10f, 0.10f, 0.18f, 0.95f); // #1A1A2E with slight transparency

        // -- Header --
        GameObject header = CreateUIElement("Header", bgObj, new Vector2(0, 850), new Vector2(960, 100));

        Text titleText = CreateText("Title", header, new Vector2(-200, 0), new Vector2(500, 100), "Météo - Rennes", 70, TextAnchor.MiddleLeft);
        titleText.fontStyle = FontStyle.Bold;

        Text lastUpdText = CreateText("LastUpdated", header, new Vector2(250, 0), new Vector2(400, 100), "Mis à jour : --:--", 36, TextAnchor.MiddleRight);
        lastUpdText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        // -- Current Weather Section --
        // Current Icon (Centered, Large)
        GameObject iconObj = CreateUIElement("CurrentIcon", bgObj, new Vector2(0, 450), new Vector2(400, 400));
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;

        // Temperature 
        Text tempText = CreateText("Temperature", bgObj, new Vector2(0, 150), new Vector2(800, 250), "--\u00b0C", 160, TextAnchor.MiddleCenter);
        tempText.fontStyle = FontStyle.Bold;

        // Description
        Text descText = CreateText("Description", bgObj, new Vector2(0, 0), new Vector2(900, 120), "Chargement...", 64, TextAnchor.MiddleCenter);
        descText.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Apparent Temperature
        Text apparentText = CreateText("ApparentTemp", bgObj, new Vector2(0, -90), new Vector2(900, 80), "Ressenti : --\u00b0C", 48, TextAnchor.MiddleCenter);
        apparentText.color = new Color(0.65f, 0.65f, 0.65f, 1f);

        // -- Divider --
        GameObject divider = CreateUIElement("Divider", bgObj, new Vector2(0, -160), new Vector2(900, 4));
        Image dividerImg = divider.AddComponent<Image>();
        dividerImg.color = new Color(1f, 1f, 1f, 0.2f);

        // -- Details Row --
        GameObject detailsRow = CreateUIElement("DetailsRow", bgObj, new Vector2(0, -250), new Vector2(960, 150));
        HorizontalLayoutGroup detailsHlg = detailsRow.AddComponent<HorizontalLayoutGroup>();
        detailsHlg.spacing = 10f;
        detailsHlg.childAlignment = TextAnchor.MiddleCenter;
        detailsHlg.childControlWidth = true;
        detailsHlg.childControlHeight = true;

        Text humText = CreateDetailItem("Humidity", detailsRow, "Humidité:\n--%");
        Text windText = CreateDetailItem("Wind", detailsRow, "Vent:\n-- km/h");
        Text precText = CreateDetailItem("Precip", detailsRow, "Précip:\n-- mm");

        // -- Offline Indicator --
        GameObject offlineObj = CreateUIElement("OfflineIndicator", bgObj, new Vector2(0, -380), new Vector2(500, 60));
        Image offlineBg = offlineObj.AddComponent<Image>();
        offlineBg.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
        offlineObj.SetActive(false);

        Text offlineText = CreateText("OfflineText", offlineObj, Vector2.zero, new Vector2(500, 60), "Hors-ligne", 36, TextAnchor.MiddleCenter);

        // -- Forecast Section (Vertical List) --
        GameObject forecastSection = CreateUIElement("ForecastSection", bgObj, new Vector2(0, -680), new Vector2(960, 500));

        VerticalLayoutGroup vlg = forecastSection.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        // Create 7 day rows
        Text[] dayNameTexts = new Text[7];
        Text[] dayTempTexts = new Text[7];
        Image[] dayIconImages = new Image[7];

        for (int i = 0; i < 7; i++)
        {
            GameObject dayRow = CreateUIElement($"DayRow{i}", forecastSection, Vector2.zero, new Vector2(960, 70));
            LayoutElement le = dayRow.AddComponent<LayoutElement>();
            le.preferredHeight = 70;
            le.preferredWidth = 960;

            // Day name (Left)
            dayNameTexts[i] = CreateText($"DayName{i}", dayRow, new Vector2(-350, 0), new Vector2(200, 70), "---", 40, TextAnchor.MiddleLeft);
            dayNameTexts[i].fontStyle = FontStyle.Bold;

            // Day icon (Center)
            GameObject dayIconObj = CreateUIElement($"DayIcon{i}", dayRow, new Vector2(-50, 0), new Vector2(100, 100));
            Image dayIcon = dayIconObj.AddComponent<Image>();
            dayIcon.color = Color.white;
            dayIcon.preserveAspect = true;
            dayIconImages[i] = dayIcon;

            // Day temperature (Right)
            dayTempTexts[i] = CreateText($"DayTemp{i}", dayRow, new Vector2(250, 0), new Vector2(350, 70), "--\u00b0 / --\u00b0", 40, TextAnchor.MiddleRight);
            dayTempTexts[i].color = new Color(0.85f, 0.85f, 0.85f, 1f);
        }

        // -- Wire up WeatherPanelUI component --
        WeatherPanelUI panelUI = root.AddComponent<WeatherPanelUI>();

        SerializedObject serializedPanel = new SerializedObject(panelUI);
        serializedPanel.FindProperty("txtTemperature").objectReferenceValue = tempText;
        serializedPanel.FindProperty("txtApparentTemp").objectReferenceValue = apparentText;
        serializedPanel.FindProperty("txtDescription").objectReferenceValue = descText;
        serializedPanel.FindProperty("txtHumidity").objectReferenceValue = humText;
        serializedPanel.FindProperty("txtWind").objectReferenceValue = windText;
        serializedPanel.FindProperty("txtPrecipitation").objectReferenceValue = precText;
        serializedPanel.FindProperty("imgCurrentIcon").objectReferenceValue = iconImage;
        serializedPanel.FindProperty("txtLastUpdated").objectReferenceValue = lastUpdText;
        serializedPanel.FindProperty("offlineIndicator").objectReferenceValue = offlineObj;

        // Wire forecast arrays
        SerializedProperty dayNamesProp = serializedPanel.FindProperty("txtDayNames");
        dayNamesProp.arraySize = 7;
        for (int i = 0; i < 7; i++)
            dayNamesProp.GetArrayElementAtIndex(i).objectReferenceValue = dayNameTexts[i];

        SerializedProperty dayMaxMinProp = serializedPanel.FindProperty("txtDayMaxMin");
        dayMaxMinProp.arraySize = 7;
        for (int i = 0; i < 7; i++)
            dayMaxMinProp.GetArrayElementAtIndex(i).objectReferenceValue = dayTempTexts[i];

        SerializedProperty dayIconsProp = serializedPanel.FindProperty("imgDayIcons");
        dayIconsProp.arraySize = 7;
        for (int i = 0; i < 7; i++)
            dayIconsProp.GetArrayElementAtIndex(i).objectReferenceValue = dayIconImages[i];

        serializedPanel.ApplyModifiedProperties();

        // -- Wire up WeatherManager component --
        WeatherManager manager = root.AddComponent<WeatherManager>();
        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("panelUI").objectReferenceValue = panelUI;
        serializedManager.ApplyModifiedProperties();

        // Select the created object
        Selection.activeGameObject = root;
        Debug.Log("[SetupWeatherPanel] Weather panel created successfully with robust UI layout.");
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

    private static Text CreateText(string name, GameObject parent, Vector2 pos, Vector2 size, string content, int fontSize, TextAnchor alignment)
    {
        GameObject obj = CreateUIElement(name, parent, pos, size);
        Text text = obj.AddComponent<Text>();
        text.text = content;
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (f == null) f = Font.CreateDynamicFontFromOSFont("Arial", 16);
        text.font = f;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
        
        // CRUCIAL: Allow overflow so text never magically disappears if bounds are slightly too small
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        
        return text;
    }

    private static Text CreateDetailItem(string name, GameObject parent, string content)
    {
        GameObject obj = CreateUIElement(name, parent, Vector2.zero, new Vector2(300, 150));
        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.preferredWidth = 300;
        le.preferredHeight = 150;

        Text text = obj.AddComponent<Text>();
        text.text = content;
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (f == null) f = Font.CreateDynamicFontFromOSFont("Arial", 16);
        text.font = f;
        text.fontSize = 40;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }
}
