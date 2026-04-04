using UnityEngine;

public class ObjectTransformManager : MonoBehaviour
{
    private ObjectSelectionManager selectionManager;
    private GameObject selectedObject;
    private GameObject lastSelectedObject;

    [Header("Scaling Settings")]
    [Tooltip("How much to add/subtract per mouse wheel notch.")]
    public float scaleSensitivity = 0.02f;  // Smaller => finer increments

    [Tooltip("Minimum scale for each axis.")]
    public float minScale = 0.1f;

    [Tooltip("Maximum multiple of the ORIGINAL scale. E.g., 3 => 3x original size.")]
    public float maxScaleMultiple = 3f;

    private Vector3 originalScale; // The object's scale when first selected

    // Press Tab to enable scaling
    public KeyCode scaleKey = KeyCode.Tab;
    // Press Alt to enable rotation
    public KeyCode rotateKey = KeyCode.LeftAlt; // or RightAlt
    private bool isScaling = false;
    private bool isRotating = false;

    void Start()
    {
        selectionManager = GetComponent<ObjectSelectionManager>();
    }

    void Update()
    {
        // Which object is currently selected?
        selectedObject = selectionManager.selectedObject;

        if (selectedObject != null)
        {
            // If we just selected a NEW object, record its original scale
            if (selectedObject != lastSelectedObject)
            {
                originalScale = selectedObject.transform.localScale;
                lastSelectedObject = selectedObject;
            }


            // Compute scaling handles


            // Check for rotation input
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                isRotating = true;
                HandleRotation();
            }
            else
            {
                isRotating = false;
            }
        }
        else
        {
            // No object selected
            isRotating = false;
            lastSelectedObject = null;
        }
    }

    /// <summary>
    /// Scales X or Y axis using the mouse wheel,
    /// capped at [minScale, 3 * originalAxis].
    /// Unused. Better scaling logic being added now.
    /// </summary>
    void HandleScaling()
    {
        if (!isScaling || selectedObject == null) return;

        // Get mouse wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.0001f) return; // No real scroll

        // Current scale
        Vector3 currentScale = selectedObject.transform.lossyScale;

        // Decide which axis to scale: Shift => X, otherwise => Y
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld)
        {
            // Scale X-axis
            float newX = currentScale.x + (scroll * scaleSensitivity);

            // Max X is 3 * originalScale.x
            float maxX = originalScale.x * maxScaleMultiple;

            // Clamp
            newX = Mathf.Clamp(newX, minScale, maxX);

            selectedObject.transform.localScale = new Vector3(newX, currentScale.y, currentScale.z);
        }
        else
        {
            // Scale Y-axis
            float newY = currentScale.y + (scroll * scaleSensitivity);

            float maxY = originalScale.y * maxScaleMultiple;

            // Clamp
            newY = Mathf.Clamp(newY, minScale, maxY);

            selectedObject.transform.localScale = new Vector3(currentScale.x, newY, currentScale.z);
        }
    }

    /// <summary>
    /// Rotate toward mouse while Alt is held.
    /// (Same logic as before.)
    /// (Currently inaccessible through Modpack Player.)
    /// </summary>
    void HandleRotation()
    {
        if (!isRotating || selectedObject == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 dir = mousePos - selectedObject.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        selectedObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
