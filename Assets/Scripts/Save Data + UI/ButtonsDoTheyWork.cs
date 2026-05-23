using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonActivator : MonoBehaviour
{
    public string playerPrefKey;

    void Start()
    {
        //do null
    }

    void ActivateOnClick(){
        Button button = GetComponent<Button>();
        if (button != null)
        {
            if (PlayerPrefs.HasKey(playerPrefKey) && PlayerPrefs.GetInt(playerPrefKey) == 1)
            {
                ActivateButton(button);
            }
            else
            {
                DeactivateButton(button);
            }
        }
    }

    void ActivateButton(Button button)
    {
        button.interactable = true;
    }

    void DeactivateButton(Button button)
    {
        button.interactable = false;
    }
}
