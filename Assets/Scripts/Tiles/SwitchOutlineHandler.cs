using UnityEngine;

public class DetachFirstChildAndDeactivateParent : MonoBehaviour
{
    void Start()
    {
        // Check if the GameObject has at least one child
        if (transform.childCount > 0)
        {
            // Get the first child of the GameObject
            Transform firstChild = transform.GetChild(0);

            // Detach the first child by setting its parent to null
            firstChild.SetParent(null);
        }
    }
}
