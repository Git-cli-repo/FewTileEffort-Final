using UnityEngine;
using UnityEngine.UI;

public class ImageActivator : MonoBehaviour
{
    public string playerPrefKey = "examplePref";
    public Image targetImage;

    void Start()
    {
        // Check if the player preference exists and is true
        if (PlayerPrefs.HasKey(playerPrefKey) && PlayerPrefs.GetInt(playerPrefKey) == 1)
        {
            ActivateImage();
        }
    }

    void ActivateImage()
    {
        if (targetImage != null)
        {
            targetImage.color = Color.green; // Change the image color to green
        }
    }
}
