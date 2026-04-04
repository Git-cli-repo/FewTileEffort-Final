using UnityEngine;
using TMPro;  // Ensure TextMeshPro namespace is used

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    public TextMeshProUGUI timerText; // Assign a TextMeshProUGUI component in the inspector

    private float elapsedTime = 0f;
    private float lastStartTime = 0f;
    private bool isTimerRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // This keeps the instance alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Check for input to start/resume or pause the timer
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartOrResumeTimer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PauseTimer();
        }

        // Update the timer display if the timer is running
        if (isTimerRunning)
        {
            UpdateTimerDisplay();
        }
    }

    public void StartOrResumeTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            lastStartTime = Time.time;
        }
    }

    public void PauseTimer()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.time - lastStartTime;
            isTimerRunning = false;
        }
    }

    private void UpdateTimerDisplay()
    {
        // Calculate total elapsed time since the timer started, accounting for pauses
        float totalTime = elapsedTime + (Time.time - lastStartTime);
        int hours = (int)(totalTime / 3600); // Calculate hours
        int minutes = (int)((totalTime % 3600) / 60); // Calculate minutes
        int seconds = (int)(totalTime % 60); // Calculate seconds

        // Update the TextMeshPro Text to display hours, minutes, and seconds
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}

