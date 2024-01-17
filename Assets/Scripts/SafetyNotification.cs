using UnityEngine;

public class SafetyNotification : MonoBehaviour
{
    private float displayTime = 60f;  // Display duration in seconds
    private float hideTime = 5f;      // Time until the notification disappears in seconds
    private float timer;

    [SerializeField]
    private GameObject SafetyNotificationPopUp;

    public PopUpManager PopUpManager;

    void Start()
    {
        SafetyNotificationPopUp.SetActive(false);

        // Start the timer for the display duration
        timer = displayTime;
    }

    void Update()
    {
        // Reduce the timer
        timer -= Time.deltaTime;

        // Check if the display duration has elapsed
        if (timer <= 0f)
        {
            // Show the safety notification
            ShowNotification();

            // Wait for the time until the notification disappears
            if (timer <= -hideTime)
            {
                // Hide the safety notification
                HideNotification();
            }
        }
    }

    void ShowNotification()
    {

        if (!PopUpManager.AreInfoPopUpsOpen()) {

            SafetyNotificationPopUp.SetActive(true);
            Logger.Instance.LogInfo("Anderes Pop Up geöffnet");

        }

    }

    void HideNotification()
    {
        SafetyNotificationPopUp.SetActive(false);


        // Start the timer for the next display duration
        timer = displayTime;
    }
}
