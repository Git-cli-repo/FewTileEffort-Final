using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUIManager : MonoBehaviour
{
    [Header("Canvas Options")]
    [Tooltip("If true, we'll reuse an existing Canvas if found. Otherwise, create a new one.")]
    public bool useExistingCanvasIfFound = true;

    [Header("Bar Layout")]
    [Tooltip("Minimum height for each achievement bar.")]
    public float barMinHeight = 80f;

    [Tooltip("Background color of each achievement bar.")]
    public Color barBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Header("Title Text Settings")]
    public Font titleFont;       // Assign in Inspector (or leave null to use LegacyRuntime.ttf)
    public int titleFontSize = 24;
    public Color unlockedTitleColor = Color.white;
    public Color lockedTitleColor = Color.gray;

    [Header("Subtext Settings")]
    public Font subtextFont;     // Assign in Inspector (or leave null to use LegacyRuntime.ttf)
    public int subtextFontSize = 16;
    public Color unlockedSubColor = Color.white;
    public Color lockedSubColor = Color.gray;

    // Reference to the scroll view's Content transform
    private Transform scrollContent;

    void Start()
    {
        // 1) Ensure we have a Canvas
        Canvas parentCanvas = FindOrCreateCanvas();

        // 2) Create the ScrollView
        GameObject scrollViewGO = CreateScrollView(parentCanvas.transform);

        // 3) Grab the Content transform
        scrollContent = FindScrollContent(scrollViewGO);

        // 4) Configure the Content with a vertical layout so each row is stacked
        ConfigureVerticalLayout(scrollContent);

        // 5) Populate achievements in the UI
        PopulateAchievementsUI();
    }

    // ----------------------------------------------------
    // (A) Create/find a Canvas
    // ----------------------------------------------------
    private Canvas FindOrCreateCanvas()
    {
        Canvas existingCanvas = null;
        if (useExistingCanvasIfFound)
        {
            List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();
            foreach (Canvas canvas in canvases) {
                if (canvas.gameObject.name == "PersistentUI")
                {

                }
                else
                {
                    existingCanvas = canvas;
                    break;
                }
            }
        }

        if (existingCanvas == null)
        {
            // No existing canvas => create a new one
            GameObject canvasGO = new GameObject("AchievementsCanvas", 
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            existingCanvas = canvasGO.GetComponent<Canvas>();
            existingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Debug.Log("Created a new Canvas for AchievementsUIManager.");
        }
        else
        {
            Debug.Log("Using existing Canvas found in the scene.");
        }
        return existingCanvas;
    }

    // ----------------------------------------------------
    // (B) Create a basic ScrollView with a Viewport + Content
    // ----------------------------------------------------
    private GameObject CreateScrollView(Transform parent)
    {
        // Create a ScrollView object
        GameObject scrollViewGO = new GameObject(
            "AchievementsScrollView",
            typeof(RectTransform),
            typeof(ScrollRect),
            typeof(Image),
            typeof(Mask)
        );
        scrollViewGO.transform.SetParent(parent, false);

        // Stretch the ScrollView to fill the parent
        RectTransform rt = scrollViewGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Set up the ScrollRect
        ScrollRect scrollRect = scrollViewGO.GetComponent<ScrollRect>();
        scrollRect.horizontal = false; // vertical scrolling
        scrollRect.scrollSensitivity = 10f; // Implement Better Mouse Wheel Scroll

        // Set up the background + mask
        Image svImage = scrollViewGO.GetComponent<Image>();
        svImage.color = new Color(1, 1, 1, 0.1f);

        Mask svMask = scrollViewGO.GetComponent<Mask>();
        svMask.showMaskGraphic = false;

        // Create a Content child
        GameObject contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(scrollViewGO.transform, false);

        RectTransform contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 1f);
        contentRT.anchorMax = new Vector2(1f, 1f);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 600);

        scrollRect.content = contentRT;

        return scrollViewGO;
    }

    // ----------------------------------------------------
    // (C) Find the Content object
    // ----------------------------------------------------
    private Transform FindScrollContent(GameObject scrollViewGO)
    {
        return scrollViewGO.transform.Find("Content");
    }

    // ----------------------------------------------------
    // (D) Setup a VerticalLayoutGroup + ContentSizeFitter
    // so multiple bars stack vertically.
    // ----------------------------------------------------
    private void ConfigureVerticalLayout(Transform content)
    {
        var vLayout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = false;
        vLayout.childForceExpandWidth = false;
        vLayout.childForceExpandHeight = false;
        vLayout.spacing = 10f;

        var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    // ----------------------------------------------------
    // (E) Populate UI with each achievement as a "bar"
    // ----------------------------------------------------
    private void PopulateAchievementsUI()
    {
        if (scrollContent == null)
        {
            Debug.LogWarning("No scrollContent to populate!");
            return;
        }

        var allAchievements = AchievementManager.Instance.GetAllAchievements();
        Debug.Log($"Populating UI with {allAchievements.Count} achievements...");

        foreach (var achInfo in allAchievements)
        {
            CreateAchievementBar(achInfo);
        }
    }

    /// <summary>
    /// Creates a "bar" for a single achievement, stretching the entire width.
    /// The bar has a background, with Title (centered) then subtext below (centered).
    /// </summary>
    private void CreateAchievementBar(AchievementInfo info)
    {
        // Parent bar object
        GameObject barGO = new GameObject(info.achievementID + "_Bar", typeof(RectTransform));
        barGO.transform.SetParent(scrollContent, false);

        // Background
        var bgImage = barGO.AddComponent<Image>();
        bgImage.color = barBackgroundColor;

        // VerticalLayoutGroup on the bar
        var barLayout = barGO.AddComponent<VerticalLayoutGroup>();
        barLayout.childControlWidth = true;
        barLayout.childControlHeight = true;   // let it control child heights
        barLayout.childForceExpandWidth = true;
        barLayout.childForceExpandHeight = false;
        barLayout.spacing = 2f;
        barLayout.childAlignment = TextAnchor.MiddleCenter; // center them horizontally

        // Optional: enforce a minimum height for the entire bar
        var barLayoutElem = barGO.AddComponent<LayoutElement>();
        barLayoutElem.minHeight = barMinHeight;

        // locked vs unlocked
        bool unlocked = AchievementManager.Instance.IsUnlocked(info.achievementID);

        // --- 1) Title text ---
        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(Text), typeof(ContentSizeFitter));
        titleGO.transform.SetParent(barGO.transform, false);

        Text titleComp = titleGO.GetComponent<Text>();
        titleComp.font = (titleFont != null) ? titleFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleComp.fontSize = titleFontSize;
        titleComp.alignment = TextAnchor.MiddleCenter;
        titleComp.text = info.title;
        titleComp.color = unlocked ? unlockedTitleColor : lockedTitleColor;

        // The ContentSizeFitter ensures the text sets its own preferred height
        ContentSizeFitter titleFitter = titleGO.GetComponent<ContentSizeFitter>();
        titleFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        titleFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // --- 2) Subtext (Description) ---
        GameObject descGO = new GameObject("Description", typeof(RectTransform), typeof(Text), typeof(ContentSizeFitter));
        descGO.transform.SetParent(barGO.transform, false);

        Text descComp = descGO.GetComponent<Text>();
        descComp.font = (subtextFont != null) ? subtextFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descComp.fontSize = subtextFontSize;
        descComp.alignment = TextAnchor.MiddleCenter;
        descComp.text = info.description;
        descComp.color = unlocked ? unlockedSubColor : lockedSubColor;

        // Again, let the text expand vertically as needed
        ContentSizeFitter descFitter = descGO.GetComponent<ContentSizeFitter>();
        descFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        descFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
}

    public void CleanupUIAndLeave()
    {
        // We'll find the canvas
        Canvas parentCanvas = FindOrCreateCanvas();

        if (parentCanvas.gameObject.name == "PersistentUI")
        { 
            // First, we get all its children
            Transform[] allChildrenTransform = GetComponentsInChildren<Transform>(true); // true = include inactive
            List<GameObject> allChildren = new List<GameObject>();
            foreach (Transform child in allChildrenTransform)
            {
                if (child != transform) // Skip the parent itself
                {
                    GameObject childObject = child.gameObject;
                    allChildren.Add(childObject);
                }
            }

            // Iteratively, we can clear out the UI
            foreach (GameObject gob in allChildren)
            {
                Destroy(gob);
            }
        }


    }
}
