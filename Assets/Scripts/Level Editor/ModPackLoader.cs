using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using SFB;

[Serializable]
public class ModPackData
{
    public string packName;
    public string author;
    public List<string> levels; // relative paths inside the pack, e.g. "levels/01_MyLevel.lvl"
}

public class ModPackLoader : MonoBehaviour
{
    // -------------------------
    // PUBLIC HOOKS
    // -------------------------

    // 1) I will call this from somewhere (button, keybind, etc.)
    public void BeginCreateModPackFlow()
    {
        ShowCreatePackPopup(
            onSubmit: (packName, author) =>
            {
                // 3) Multi-select .lvl files
                string[] lvlPaths = PickLevelFilesMulti();
                if (lvlPaths == null || lvlPaths.Length == 0)
                {
                    Debug.Log("Mod pack creation cancelled: no levels selected.");
                    return;
                }

                // Save location for .ftemp
                string savePath = PickSavePathForPack(packName);
                if (string.IsNullOrEmpty(savePath))
                {
                    Debug.Log("Mod pack creation cancelled: no save path selected.");
                    return;
                }

                try
                {
                    CreateModPack(packName, author, lvlPaths, savePath);
                    Debug.Log("Created mod pack: " + savePath);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to create mod pack: " + e);
                }
            }
        );
    }

    // Optional simple starter
    public void StartModPack()
    {
        var extensions = new[]
        {
            new ExtensionFilter("FTE Mod Pack", "ftemp"),
            new ExtensionFilter("All Files", "*")
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load Mod Pack", "", extensions, false);
        if (paths == null || paths.Length == 0) return;

        LoadModPackFromPath(paths[0]);
    }

    // -------------------------
    // CREATE PACK IMPLEMENTATION
    // -------------------------

    private string[] PickLevelFilesMulti()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Level Files", "lvl"),
            new ExtensionFilter("All Files", "*")
        };

        // Multi-select = true
        string[] paths = StandaloneFileBrowser.OpenFilePanel(
            "Select .lvl files (multi-select)",
            "",
            extensions,
            true
        );

        return (paths != null && paths.Length > 0) ? paths : Array.Empty<string>();
    }

    private string PickSavePathForPack(string packName)
    {
        var extensions = new[]
        {
            new ExtensionFilter("FTE Mod Pack", "ftemp"),
            new ExtensionFilter("All Files", "*")
        };

        // SaveFilePanel returns a full path. If the plugin appends extension weirdly, we normalize later.
        string path = StandaloneFileBrowser.SaveFilePanel(
            "Save Mod Pack As...",
            "",
            SanitizeFileName(packName),
            extensions
        );

        if (string.IsNullOrEmpty(path)) return "";

        // Ensure it ends with .ftemp (some platforms may omit extension)
        if (!path.EndsWith(".ftemp", StringComparison.OrdinalIgnoreCase))
            path += ".ftemp";

        return path;
    }

    private void CreateModPack(string packName, string author, string[] levelFilePaths, string outFtempPath)
    {
        if (levelFilePaths == null || levelFilePaths.Length == 0)
            throw new ArgumentException("No levels provided.");

        // Build a temp working folder
        string tempRoot = Path.Combine(Application.temporaryCachePath, "FTE_ModPackBuild");
        string buildId = DateTime.UtcNow.Ticks.ToString();
        string buildDir = Path.Combine(tempRoot, buildId);

        string levelsDir = Path.Combine(buildDir, "levels");
        Directory.CreateDirectory(levelsDir);

        // Copy selected levels into levels/
        List<string> relativeLevelPaths = new List<string>();
        for (int i = 0; i < levelFilePaths.Length; i++)
        {
            string src = levelFilePaths[i];
            if (!File.Exists(src))
                throw new FileNotFoundException("Level file not found: " + src);

            // Keep ordering as selected by file browser
            // Name them safely and uniquely inside the pack
            string baseName = SanitizeFileName(Path.GetFileNameWithoutExtension(src));
            string destFileName = $"{(i + 1).ToString("D2")}_{baseName}.lvl";
            string dest = Path.Combine(levelsDir, destFileName);

            File.Copy(src, dest, true);

            relativeLevelPaths.Add(Path.Combine("levels", destFileName).Replace("\\", "/"));
        }

        // Create pack.json
        ModPackData pack = new ModPackData
        {
            packName = packName,
            author = author,
            levels = relativeLevelPaths
        };

        string packJson = JsonConvert.SerializeObject(pack, Formatting.Indented);
        File.WriteAllText(Path.Combine(buildDir, "pack.json"), packJson);

        // Zip buildDir to a temp zip, then rename/copy to .ftemp
        string tempZip = Path.Combine(tempRoot, buildId + ".zip");
        if (File.Exists(tempZip)) File.Delete(tempZip);

        ZipFile.CreateFromDirectory(buildDir, tempZip, System.IO.Compression.CompressionLevel.Optimal, false);

        // Ensure target dir exists :(
        string outDir = Path.GetDirectoryName(outFtempPath);
        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
            Directory.CreateDirectory(outDir);

        File.Copy(tempZip, outFtempPath, true);

        // Cleanup (best-effort)
        try
        {
            Directory.Delete(buildDir, true);
            File.Delete(tempZip);
        }
        catch { /* too laxy to catch cleanup fialure */ }
    }

    private string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "UntitledPack";
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    // -------------------------
    // DYNAMIC POPUP UI (NO PREFABS)
    // -------------------------

    private void ShowCreatePackPopup(Action<string, string> onSubmit)
    {
        // Find or create a Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject cgo = new GameObject("RuntimeCanvas");
            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cgo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cgo.AddComponent<GraphicRaycaster>();
        }

        // Root overlay
        GameObject overlay = new GameObject("ModPackPopup_Overlay");
        overlay.transform.SetParent(canvas.transform, false);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.65f);
        RectTransform overlayRt = overlay.GetComponent<RectTransform>();
        overlayRt.anchorMin = Vector2.zero;
        overlayRt.anchorMax = Vector2.one;
        overlayRt.offsetMin = Vector2.zero;
        overlayRt.offsetMax = Vector2.zero;

        // Panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(overlay.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.12f, 0.12f, 0.12f, 0.98f);
        RectTransform panelRt = panel.GetComponent<RectTransform>();
        panelRt.sizeDelta = new Vector2(520, 320);
        panelRt.anchorMin = panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.anchoredPosition = Vector2.zero;

        // Title
        TMP_Text title = CreateTMPText(panel.transform, "Create Mod Pack", 24, TextAlignmentOptions.MidlineLeft);
        RectTransform titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0, 1);
        titleRt.anchorMax = new Vector2(1, 1);
        titleRt.pivot = new Vector2(0.5f, 1);
        titleRt.anchoredPosition = new Vector2(0, -18);
        titleRt.sizeDelta = new Vector2(-40, 40);
        title.margin = new Vector4(20, 0, 20, 0);

        // Pack Name label + input
        TMP_Text nameLbl = CreateTMPText(panel.transform, "Pack Name", 16, TextAlignmentOptions.MidlineLeft);
        PositionRow(nameLbl.rectTransform, topY: -70);

        TMP_InputField nameField = CreateTMPInput(panel.transform, "My Cool Pack");
        PositionRow(nameField.GetComponent<RectTransform>(), topY: -100);

        // Author label + input
        TMP_Text authorLbl = CreateTMPText(panel.transform, "Author", 16, TextAlignmentOptions.MidlineLeft);
        PositionRow(authorLbl.rectTransform, topY: -155);

        TMP_InputField authorField = CreateTMPInput(panel.transform, "Your Name");
        PositionRow(authorField.GetComponent<RectTransform>(), topY: -185);

        // Buttons
        Button cancelBtn = CreateButton(panel.transform, "Cancel");
        RectTransform cancelRt = cancelBtn.GetComponent<RectTransform>();
        cancelRt.anchorMin = new Vector2(0, 0);
        cancelRt.anchorMax = new Vector2(0, 0);
        cancelRt.pivot = new Vector2(0, 0);
        cancelRt.anchoredPosition = new Vector2(20, 20);
        cancelRt.sizeDelta = new Vector2(140, 44);

        Button createBtn = CreateButton(panel.transform, "Create...");
        RectTransform createRt = createBtn.GetComponent<RectTransform>();
        createRt.anchorMin = new Vector2(1, 0);
        createRt.anchorMax = new Vector2(1, 0);
        createRt.pivot = new Vector2(1, 0);
        createRt.anchoredPosition = new Vector2(-20, 20);
        createRt.sizeDelta = new Vector2(160, 44);

        cancelBtn.onClick.AddListener(() =>
        {
            Destroy(overlay);
        });

        createBtn.onClick.AddListener(() =>
        {
            string packName = nameField.text?.Trim();
            string author = authorField.text?.Trim();

            if (string.IsNullOrWhiteSpace(packName))
                packName = "Untitled Pack";

            if (string.IsNullOrWhiteSpace(author))
                author = "Unknown";

            Destroy(overlay);
            onSubmit?.Invoke(packName, author);
        });
    }

    private TMP_Text CreateTMPText(Transform parent, string text, int fontSize, TextAlignmentOptions align)
    {
        GameObject gob = new GameObject("TMP_Text");
        gob.transform.SetParent(parent, false);
        TMP_Text tmp = gob.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = align;

        RectTransform rt = tmp.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.sizeDelta = new Vector2(-40, 28);
        rt.anchoredPosition = Vector2.zero;

        return tmp;
    }

    private TMP_InputField CreateTMPInput(Transform parent, string placeholderText)
    {
        // Root
        GameObject root = new GameObject("TMP_Input");
        root.transform.SetParent(parent, false);
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(1f, 1f, 1f, 0.08f);

        RectTransform rt = root.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.sizeDelta = new Vector2(-40, 40);

        // Text (child)
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(root.transform, false);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform textRt = text.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0, 0);
        textRt.anchorMax = new Vector2(1, 1);
        textRt.offsetMin = new Vector2(12, 6);
        textRt.offsetMax = new Vector2(-12, -6);

        // Placeholder (child)
        GameObject phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(root.transform, false);
        TextMeshProUGUI ph = phGO.AddComponent<TextMeshProUGUI>();
        ph.text = placeholderText;
        ph.fontSize = 18;
        ph.color = new Color(1f, 1f, 1f, 0.35f);
        ph.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform phRt = ph.GetComponent<RectTransform>();
        phRt.anchorMin = new Vector2(0, 0);
        phRt.anchorMax = new Vector2(1, 1);
        phRt.offsetMin = new Vector2(12, 6);
        phRt.offsetMax = new Vector2(-12, -6);

        // InputField
        TMP_InputField input = root.AddComponent<TMP_InputField>();
        input.textComponent = text;
        input.placeholder = ph;

        return input;
    }

    private Button CreateButton(Transform parent, string label)
    {
        GameObject go = new GameObject("Button_" + label);
        go.transform.SetParent(parent, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.12f);

        Button btn = go.AddComponent<Button>();

        // Label
        TMP_Text t = CreateTMPText(go.transform, label, 18, TextAlignmentOptions.Center);
        RectTransform trt = t.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        t.margin = Vector4.zero;

        return btn;
    }

    private void PositionRow(RectTransform rt, float topY)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, topY);
    }

    // -------------------------
    // LOAD (same idea as earlier)
    // -------------------------

    private void LoadModPackFromPath(string packPath)
    {
        if (!File.Exists(packPath))
        {
            Debug.LogError("Mod pack not found: " + packPath);
            return;
        }

        string extractRoot = Path.Combine(Application.persistentDataPath, "ModPacks");
        Directory.CreateDirectory(extractRoot);

        string packFolderName = Path.GetFileNameWithoutExtension(packPath);
        string extractPath = Path.Combine(extractRoot, packFolderName);

        if (Directory.Exists(extractPath))
            Directory.Delete(extractPath, true);
        Directory.CreateDirectory(extractPath);

        string tempZipPath = packPath + ".zip";
        File.Copy(packPath, tempZipPath, true);
        ZipFile.ExtractToDirectory(tempZipPath, extractPath);
        File.Delete(tempZipPath);

        string packJsonPath = Path.Combine(extractPath, "pack.json");
        if (!File.Exists(packJsonPath))
        {
            Debug.LogError("pack.json missing in mod pack");
            return;
        }

        ModPackData packData = JsonConvert.DeserializeObject<ModPackData>(File.ReadAllText(packJsonPath));
        if (packData?.levels == null || packData.levels.Count == 0)
        {
            Debug.LogError("Invalid mod pack.json");
            return;
        }

        List<string> resolved = new List<string>();
        foreach (string rel in packData.levels)
        {
            string full = Path.Combine(extractPath, rel);
            if (!File.Exists(full))
            {
                Debug.LogError("Missing level file: " + full);
                return;
            }
            resolved.Add(full);
        }

        var run = CompleteAchievementsRunManager.Instance;
        run.isPlayingModPack = true;
        run.currentLevelIndex = 0;
        run.levelPaths = resolved;
        run.numberOfLevels = resolved.Count;

        LevelSaveManager lsm = FindAnyObjectByType<LevelSaveManager>();
        if (lsm == null)
        {
            Debug.LogError("LevelSaveManager not found in scene; cannot load first level.");
            return;
        }

        lsm.LoadLevelFromPath(resolved[0]);
        GameObject playerObject = GameObject.Find("Player");
        GameObject spawnPoint = GameObject.Find("StartPos");
        playerObject.transform.position = spawnPoint.transform.position;
    }
}
