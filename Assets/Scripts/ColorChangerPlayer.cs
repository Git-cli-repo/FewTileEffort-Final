using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PlayerColorChanger : MonoBehaviour
{
    public Renderer playerRenderer; // Renderer to change player's color

    private string colorKey = "playerColor"; // tracker.json key for storing the color

    private GameObject colorPopup; // Dynamic popup
    private TMP_InputField hexInputField;
    private GameObject canvas;

    void Start()
    {
        // Create a Canvas for UI if it doesn't exist
        canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // Load and apply the stored color if it exists
        if (File.Exists(Path.Combine(Application.persistentDataPath + "tracker.json")))
        {
            string savedColor = AchievementManager.Instance.ReadFromFile("tracker.json", colorKey, true);
            ChangePlayerColor(savedColor);
        }
    }

    void Update()
    {
        // Show popup when pressing the number 5 key
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleColorPopup();
        }
    }

    // Toggle the popup visibility and generate it if it doesn't exist
    public void ToggleColorPopup()
    {
        if (colorPopup == null)
        {
            GenerateColorPopup();
        }
        else
        {
            colorPopup.SetActive(!colorPopup.activeSelf); // Show/hide the popup
        }
    }

    // Function to generate the color popup dynamically
    void GenerateColorPopup()
    {
        // Create the popup background (panel)
        colorPopup = new GameObject("ColorPopup");
        RectTransform popupRect = colorPopup.AddComponent<RectTransform>();
        popupRect.SetParent(canvas.transform, false);
        popupRect.sizeDelta = new Vector2(300, 150);
        colorPopup.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f); // Semi-transparent black background

        // Create the input field for hex color input
        GameObject inputFieldObject = new GameObject("HexInputField");
        RectTransform inputFieldRect = inputFieldObject.AddComponent<RectTransform>();
        inputFieldRect.SetParent(colorPopup.transform, false);
        inputFieldRect.anchoredPosition = new Vector2(0, 20);
        inputFieldRect.sizeDelta = new Vector2(250, 40);

        hexInputField = inputFieldObject.AddComponent<TMP_InputField>();
        TextMeshProUGUI textComponent = inputFieldObject.AddComponent<TextMeshProUGUI>();
        hexInputField.textComponent = textComponent;
        textComponent.text = "Enter Hex Code";
        textComponent.fontSize = 24;
        textComponent.color = Color.white;

        // Create the Submit button
        GameObject submitButtonObject = new GameObject("SubmitButton");
        RectTransform submitButtonRect = submitButtonObject.AddComponent<RectTransform>();
        submitButtonRect.SetParent(colorPopup.transform, false);
        submitButtonRect.anchoredPosition = new Vector2(0, -50);
        submitButtonRect.sizeDelta = new Vector2(100, 40);

        Button submitButton = submitButtonObject.AddComponent<Button>();
        TextMeshProUGUI buttonText = submitButtonObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Submit";
        buttonText.fontSize = 24;
        buttonText.color = Color.white;

        submitButton.onClick.AddListener(SubmitHexColor);
    }

    // Function to be called when the "Submit" button is pressed
    public void SubmitHexColor()
    {
        string hexCode = hexInputField.text;

        // Validate and apply the hex color
        if (IsValidHexCode(hexCode))
        {
            ChangePlayerColor(hexCode);
            AchievementManager.Instance.WriteToFile("tracker.json", hexCode, colorKey); // Save to tracker.json
            colorPopup.SetActive(false); // Hide the popup after submitting
        }
        else
        {
            Debug.LogError("Invalid Hex Code");
        }
    }

    // Validate if the input string is a valid hex code
    private bool IsValidHexCode(string hexCode)
    {
        if (string.IsNullOrEmpty(hexCode) || hexCode.Length != 6) return false;

        Color tempColor;
        return ColorUtility.TryParseHtmlString("#" + hexCode, out tempColor);
    }

    // Change the player's color using the hex code
    public void ChangePlayerColor(string hexCode)
    {
        List<float> rgb = new List<float>();
        for (int i = 0; i < hexCode.Split(",").ToList().Count; i++)
        {
            rgb.Add(float.Parse(hexCode.Split(",").ToList()[i]));
        }
        Color newColor = new Color(rgb[0], rgb[1], rgb[2], 1f);
        playerRenderer.material.color = newColor;
    }
}
