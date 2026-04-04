using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ObjectSelectionManager : MonoBehaviour
{
    // Selected object for moving and deletion
    public GameObject selectedObject = null;
    public GameObject lastSelectedObject = null;

    // The outline object
    private GameObject outlineObject = null;

    // Outline properties
    public Color outlineColor = new Color(1f, 0.5f, 0f, 1f); // "Selected" orange color
    public float outlineThickness = 0.1f; // How thick the outline is

    // Grid snapping variables (optional)
    public bool isGridSnapping = false;
    public float gridSize = 1f; // Size of the grid cells

    // Scaling variables
    public float offsetAmount = 0f; // Offset amount for handle positions
    public GameObject handlePrefab; // Prefab for scaling handles
    private float w0, h0, s0x, s0y, L, R, B, T; // Reference floats for resizing
    public bool isResizing = false;
    public bool useWScale = true;
    private readonly List<GameObject> activeHandles = new List<GameObject>();

    private bool isHandleDrag = false;
    private HandleDataContainer.HandleEdgeType activeEdgeType;



void Update()
{
    if (Input.GetMouseButtonDown(0) && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
    {
        // If we clicked a handle or are reasonably-ish near one, begin resizing and skip selection
        if (selectedObject != null && TryGetHandleNearMouse(0.15f, out GameObject hObj))
        {
            activeEdgeType = hObj.GetComponent<HandleDataContainer>().edgeType;
            isHandleDrag = true;

            if (!isResizing) BeginResizeCache();

            Debug.LogError(
                $"[RESIZE START] edge={activeEdgeType} | " +
                $"pos={selectedObject.transform.position} | " +
                $"scale={selectedObject.transform.localScale}"
            );

        }
        else
        {
            HandleSelection();
        }
    }

    // Mouse held: either resize (locked), or drag object
    if (Input.GetMouseButton(0) && selectedObject != null && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
    {
        if (isHandleDrag) ResizeObjectLocked(activeEdgeType);
        else if (TryGetHoveredObject(LoadMode.LookForGameObject, out _, out GameObject g) && g == selectedObject) DragSelectedObject();
    }

    if (Input.GetMouseButtonUp(0)  && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
    {
        isHandleDrag = false;
        isResizing = false;
        Debug.LogError(
            $"[RESIZE END] edge={activeEdgeType} | " +
            $"final scale={lastSelectedObject.transform.localScale}"
        );
    }

    if(Input.GetKeyDown(KeyCode.Backspace) && selectedObject != null && !CompleteAchievementsRunManager.Instance.isPlayingModPack)
    {
        DeleteSelectedObject();        
    }

    
}

    // Handle selecting or deselecting objects
    void HandleSelection()
    {
        if (TryGetHoveredObject(LoadMode.LookForGameObject, out RaycastHit2D hit, out _) && hit.collider != null)
        {
            // Select the clicked object
            GameObject clickedObject = hit.collider.gameObject;
            if (selectedObject != null)
            {
                RemoveOutline(); // Remove outline from previously selected object
                ClearHandles(); // Remove all scaling handles here
            }
            selectedObject = clickedObject;
            lastSelectedObject = clickedObject;
            CreateOutline(selectedObject); // Create outline for the new selected object
            Collider2D col = selectedObject.GetComponent<Collider2D>();
            Vector2 C = col.bounds.center;
            Vector2 E = col.bounds.extents;
            float d = offsetAmount;

            // Right Handle
            Vector2 vec = new Vector2(C.x + E.x + d, C.y);
            GameObject handleRight = Instantiate(handlePrefab, vec, Quaternion.identity);
            HandleDataContainer h = handleRight.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.Right;
            activeHandles.Add(handleRight);

            // Left Handle
            vec = new Vector2(C.x - E.x - d , C.y);
            GameObject handleLeft = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleLeft.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.Left;
            activeHandles.Add(handleLeft);

            // Top Handle
            vec = new Vector2(C.x , C.y + E.y + d);
            GameObject handleTop = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleTop.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.Top;
            activeHandles.Add(handleTop);

            // Bottom Handle
            vec = new Vector2(C.x , C.y - E.y - d);
            GameObject handleBottom = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleBottom.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.Bottom;
            activeHandles.Add(handleBottom);

            // Top-Right Handle
            vec = new Vector2(C.x + E.x + d , C.y + E.y + d);
            GameObject handleTopRight = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleTopRight.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.TopRight;
            activeHandles.Add(handleTopRight);

            // Top-Left Handle
            vec = new Vector2(C.x - E.x - d , C.y + E.y + d);
            GameObject handleTopLeft = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleTopLeft.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.TopLeft;
            activeHandles.Add(handleTopLeft);

            // Bottom-Right Handle
            vec = new Vector2(C.x + E.x + d , C.y - E.y - d);
            GameObject handleBottomRight = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleBottomRight.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.BottomRight;
            activeHandles.Add(handleBottomRight);

            // Bottom-Left Handle
            vec = new Vector2(C.x - E.x - d , C.y - E.y - d);
            GameObject handleBottomLeft = Instantiate(handlePrefab, vec, Quaternion.identity);
            h = handleBottomLeft.GetComponent<HandleDataContainer>();
            h.edgeType = HandleDataContainer.HandleEdgeType.BottomLeft;
            activeHandles.Add(handleBottomLeft);
        }
        else
        {
            // Deselect if clicking outside of objects
            if (selectedObject != null)
            {
                // If clicking a handle, do NOT change selection
                if (TryGetHoveredObject(LoadMode.LookForHandler, out _, out _))
                {
                    return;
                }
                RemoveOutline(); // Remove outline from the current selected object
                ClearHandles(); // Remove all scaling handles here
                selectedObject = null;
            }
        }
    }

    // Create an outline around the selected object
    void CreateOutline(GameObject obj)
    {
        // Create an empty GameObject to hold the outline
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(obj.transform, false); // Attach to the selected object

        // Copy the SpriteRenderer and apply the outline color
        SpriteRenderer originalRenderer = obj.GetComponent<SpriteRenderer>();
        SpriteRenderer outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = originalRenderer.sprite; // Copy the sprite
        outlineRenderer.color = outlineColor; // Set outline color

        // Adjust the outline's size to make it look like a border
        outlineObject.transform.localScale = obj.transform.localScale + new Vector3(outlineThickness, outlineThickness, 0);

        // Render the outline behind the original object
        outlineRenderer.sortingOrder = originalRenderer.sortingOrder - 1;
    }

    void ClearHandles()
    {
        foreach(Transform child in activeHandles.Select(h => h.transform))
        {
            if(child.gameObject.TryGetComponent<HandleDataContainer>(out HandleDataContainer h))
            {
                Destroy(child.gameObject);
            }
        }
        
    }

    // Remove the outline from the currently selected object
    void RemoveOutline()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject); // Remove the outline GameObject
        }
    }

  

    // Drag the selected object
    void DragSelectedObject()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mousePosition.z = 0;

        if (isGridSnapping)
        {
            mousePosition = new Vector3(
                Mathf.Round(mousePosition.x / gridSize) * gridSize,
                Mathf.Round(mousePosition.y / gridSize) * gridSize,
                0
            );
        }

        selectedObject.transform.position = mousePosition;
        UpdateHandlePositionsWorld();
    }

    void ResizeObjectLocked(HandleDataContainer.HandleEdgeType edgeType)
    {
        Debug.LogError(
            $"[RESIZE TICK] edge={edgeType} | " +
            $"mouse={Camera.main.ScreenToWorldPoint(Input.mousePosition)}"
        );

        Vector3 M = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        M.z = 0;

        if (isGridSnapping)
        {
            M = new Vector3(
                Mathf.Round(M.x / gridSize) * gridSize,
                Mathf.Round(M.y / gridSize) * gridSize,
                0
            );
        }

        Transform P = lastSelectedObject.transform;
        float W, H;

        switch (edgeType)
        {
            case HandleDataContainer.HandleEdgeType.Right:
                W = M.x - L;
                P.position = new Vector2((M.x + L) / 2, P.position.y);
                P.localScale = new Vector2(s0x * (W / w0), P.localScale.y);
                break;

            case HandleDataContainer.HandleEdgeType.Left:
                W = R - M.x;
                P.position = new Vector2((R + M.x) / 2, P.position.y);
                P.localScale = new Vector2(s0x * (W / w0), P.localScale.y);
                break;

            case HandleDataContainer.HandleEdgeType.Bottom:
                H = T - M.y;
                P.position = new Vector2(P.position.x, (T + M.y) / 2);
                P.localScale = new Vector2(P.localScale.x, s0y * (H / h0));
                break;

            case HandleDataContainer.HandleEdgeType.Top:
                H = M.y - B;
                P.position = new Vector2(P.position.x, (M.y + B) / 2);
                P.localScale = new Vector2(P.localScale.x, s0y * (H / h0));
                break;

            case HandleDataContainer.HandleEdgeType.TopRight:
                W = M.x - L; H = M.y - B;
                P.position = new Vector2((M.x + L) / 2, (M.y + B) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;

            case HandleDataContainer.HandleEdgeType.TopLeft:
                W = R - M.x; H = M.y - B;
                P.position = new Vector2((R + M.x) / 2, (M.y + B) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;

            case HandleDataContainer.HandleEdgeType.BottomRight:
                W = M.x - L; H = T - M.y;
                P.position = new Vector2((M.x + L) / 2, (T + M.y) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;

            case HandleDataContainer.HandleEdgeType.BottomLeft:
                W = R - M.x; H = T - M.y;
                P.position = new Vector2((R + M.x) / 2, (T + M.y) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;
        }

        UpdateHandlePositionsWorld(); // Update Handle Positions for weird sizing logic bugs
    }


    // Old, Unused method
    void ResizeObject()
    {
        Vector3 M = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        M.z = 0;

        if (isGridSnapping)
        {
            M = new Vector3(
                Mathf.Round(M.x / gridSize) * gridSize,
                Mathf.Round(M.y / gridSize) * gridSize,
                0
            );
        }

        if(!TryGetHoveredObject(LoadMode.LookForHandler, out RaycastHit2D rayHit, out GameObject gob)) return;
        HandleDataContainer.HandleEdgeType edgeType = gob.GetComponent<HandleDataContainer>().edgeType;
        if(!isResizing)
        {
            Collider2D coll = selectedObject.GetComponent<Collider2D>();
            Vector2 C = coll.bounds.center;
            Vector2 E = coll.bounds.extents;
            w0 = selectedObject.GetComponent<Collider2D>().bounds.size.x;
            h0 = selectedObject.GetComponent<Collider2D>().bounds.size.y;
            s0x = selectedObject.transform.localScale.x;
            s0y = selectedObject.transform.localScale.y;
            isResizing = true;
            L = C.x - E.x;
            R = C.x + E.x;
            B = C.y - E.y;
            T = C.y + E.y;
        }

        Collider2D col = selectedObject.GetComponent<Collider2D>();
        Transform P = selectedObject.transform;
        float W = 0f;
        float H = 0f;
        switch (edgeType)
        {
            case HandleDataContainer.HandleEdgeType.Right:
                W = M.x - L;
                P.position = new Vector2((M.x + L) / 2, P.position.y);
                P.localScale = new Vector2(s0x * (W / w0), P.localScale.y);
                break;
            case HandleDataContainer.HandleEdgeType.Left:
                W = R - M.x;
                P.position = new Vector2((R + M.x) / 2, P.position.y);
                P.localScale = new Vector2(s0x * (W / w0), P.localScale.y);
                break;
            case HandleDataContainer.HandleEdgeType.Bottom:
                H = T - M.y;
                P.position = new Vector2(P.position.x, (T + M.y) / 2);
                P.localScale = new Vector2(P.localScale.x, s0y * (H / h0));
                break;
            case HandleDataContainer.HandleEdgeType.Top:
                H = M.y - B;
                P.position = new Vector2(P.position.x, (M.y + B) / 2);
                P.localScale = new Vector2(P.localScale.x, s0y * (H / h0));
                break;
            case HandleDataContainer.HandleEdgeType.TopRight:
                W = M.x - L;
                H = M.y - B;

                P.position = new Vector2((M.x + L) / 2, (M.y + B) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;
            case HandleDataContainer.HandleEdgeType.TopLeft:
                W = R - M.x;
                H = M.y - B;

                P.position = new Vector2((R + M.x) / 2, (M.y + B) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;  
            case HandleDataContainer.HandleEdgeType.BottomRight:
                W = M.x - L;
                H = T - M.y;

                P.position = new Vector2((M.x + L) / 2, (T + M.y) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));
                break;
            case HandleDataContainer.HandleEdgeType.BottomLeft:
                W = R - M.x;
                H = T - M.y;

                P.position = new Vector2((R + M.x) / 2, (T + M.y) / 2);
                P.localScale = new Vector2(s0x * (W / w0), s0y * (H / h0));

                break;
        }

        // It is unknown if the below code is needed.
        /*
        if (isGridSnapping)
        {
            M = new Vector3(
                Mathf.Round(M.x / gridSize) * gridSize,
                Mathf.Round(M.y / gridSize) * gridSize,
                0
            );
        }

        selectedObject.transform.position = M;
        */
    }
    

    // Delete the selected object with a right-click, but only if hovering over it
    void DeleteSelectedObject()
    {
        if (selectedObject != null)
        {
            Destroy(selectedObject);
            RemoveOutline(); // Also remove the outline when deleting the object
            selectedObject = null;
            lastSelectedObject = null;
        }
    }

    // Check if the mouse is hovering over the selected object
    // TODO - Deprecate this function in favor of GetHoveredObject()
    bool IsMouseHoveringOverObject(GameObject obj)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        return hit.collider != null && hit.collider.gameObject == obj;
    }

    public enum LoadMode
    {
        LookForGameObject,
        LookForHandler
    }

    public bool TryGetHoveredObject(LoadMode mode, out RaycastHit2D rayHit, out GameObject gob)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<RaycastHit2D> hit = Physics2D.RaycastAll(mousePosition, Vector2.zero).ToList();
        List<RaycastHit2D> usableHits = new List<RaycastHit2D>(); 
        if(mode == LoadMode.LookForGameObject)
        {
            foreach(RaycastHit2D rh in hit)
            {
                if (rh.collider != null && rh.collider.gameObject != null && !rh.collider.gameObject.TryGetComponent<HandleDataContainer>(out HandleDataContainer h))
                {
                    usableHits.Add(rh);
                }
            }
        } else if(mode == LoadMode.LookForHandler){
            foreach(RaycastHit2D rh in hit)
            {
                if (rh.collider != null && rh.collider.gameObject != null && rh.collider.gameObject.TryGetComponent<HandleDataContainer>(out HandleDataContainer h))
                {
                    usableHits.Add(rh);
                }
            }
        }
        
        bool toreturn;
        rayHit = default;
        gob = null;
        if(usableHits.Count <= 0) toreturn = false;
        else {
            toreturn = true;
            rayHit = usableHits[0];
            gob = usableHits[0].collider.gameObject;
        }
        return toreturn;
    }

    void UpdateHandlePositionsWorld()
    {
        if (selectedObject == null) return;

        Collider2D col = selectedObject.GetComponent<Collider2D>();
        if (col == null) return;

        Vector2 C = col.bounds.center;
        Vector2 E = col.bounds.extents;
        float d = offsetAmount;

        // If any got destroyed, clean the list as we go
        for (int i = activeHandles.Count - 1; i >= 0; i--)
        {
            GameObject hObj = activeHandles[i];

            // NOTE: This line may cause issues later
            // TODO: Add to IssueTracker.tfrt > Warn1
            if (hObj == null)
            {
                activeHandles.RemoveAt(i);
                continue;
            }

            var h = hObj.GetComponent<HandleDataContainer>();
            if (h == null) continue;

            Vector2 pos;
            switch (h.edgeType)
            {
                case HandleDataContainer.HandleEdgeType.Right:       pos = new Vector2(C.x + E.x + d, C.y); break;
                case HandleDataContainer.HandleEdgeType.Left:        pos = new Vector2(C.x - E.x - d, C.y); break;
                case HandleDataContainer.HandleEdgeType.Top:         pos = new Vector2(C.x, C.y + E.y + d); break;
                case HandleDataContainer.HandleEdgeType.Bottom:      pos = new Vector2(C.x, C.y - E.y - d); break;

                case HandleDataContainer.HandleEdgeType.TopRight:    pos = new Vector2(C.x + E.x + d, C.y + E.y + d); break;
                case HandleDataContainer.HandleEdgeType.TopLeft:     pos = new Vector2(C.x - E.x - d, C.y + E.y + d); break;
                case HandleDataContainer.HandleEdgeType.BottomRight: pos = new Vector2(C.x + E.x + d, C.y - E.y - d); break;
                case HandleDataContainer.HandleEdgeType.BottomLeft:  pos = new Vector2(C.x - E.x - d, C.y - E.y - d); break;

                default: pos = hObj.transform.position; break;
            }

            hObj.transform.position = pos;
        }
    }

    void BeginResizeCache()
    {
        Collider2D coll = selectedObject.GetComponent<Collider2D>();
        Vector2 C = coll.bounds.center;
        Vector2 E = coll.bounds.extents;

        w0 = coll.bounds.size.x;
        h0 = coll.bounds.size.y;

        s0x = selectedObject.transform.localScale.x;
        s0y = selectedObject.transform.localScale.y;

        L = C.x - E.x;
        R = C.x + E.x;
        B = C.y - E.y;
        T = C.y + E.y;

        isResizing = true;
    }

    bool TryGetHandleNearMouse(float radius, out GameObject handleObj)
    {
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(m.x, m.y);

        var cols = Physics2D.OverlapCircleAll(p, radius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] != null && cols[i].TryGetComponent<HandleDataContainer>(out _))
            {
                handleObj = cols[i].gameObject;
                return true;
            }
        }

        handleObj = null;
        return false;
    }


}
