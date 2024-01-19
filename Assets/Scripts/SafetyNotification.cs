using UnityEngine;

/*
 * Script Name: SafetyNotification.cs
 * Author: Tabea Spaenich
 * Date: November 25, 2023
 * Description: Manages the display and hiding of safety notifications.
 */

public class SafetyNotification : MonoBehaviour
{
    // Display duration in seconds
    private float displayTime = 60f;

    // Time until the notification disappears in seconds
    private float hideTime = 5f;

    private float timer;

    // Reference to the safety notification popup GameObject
    [SerializeField] private GameObject safetyNotificationPopUp;

    // Reference to the PopUpManager script
    public PopUpManager PopUpManager;

    void Start()
    {
        // Set the safety notification popup as inactive initially
        safetyNotificationPopUp.SetActive(false);

        // Start the timer for the display duration
        timer = displayTime;
    }

    void Update()
    {
        // Reduce the timer based on real-time
        timer -= Time.deltaTime;

        // Check if the display duration has elapsed
        if (timer <= 0f)
        {
            // Show the safety notification if no other info pop-ups are open
            ShowNotification();

            // Wait for the time until the notification disappears
            if (timer <= -hideTime)
            {
                // Hide the safety notification
                HideNotification();
            }
        }
    }

    // Displays the safety notification if no other info pop-ups are open
    void ShowNotification()
    {
        if (!PopUpManager.AreInfoPopUpsOpen())
        {
            safetyNotificationPopUp.SetActive(true);
        }
    }

    // Hides the safety notification and resets the timer for the next display duration
    void HideNotification()
    {
        // Set the safety notification popup as inactive
        safetyNotificationPopUp.SetActive(false);

        // Start the timer for the next display duration
        timer = displayTime;
    }
}
