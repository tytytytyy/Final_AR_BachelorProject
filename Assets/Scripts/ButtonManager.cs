using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/*
 * Script Name: ButtonManager.cs
 * Author: Tabea Spaenich
 * Date: November 28, 2023
 * Description: Manages the Buttons and control of information popup windows.
 */
public class ButtonManager : MonoBehaviour
{
    // Reference to MaterialButton
    [SerializeField]
    private Button MaterialButton;

    // Reference to CameraButton
    [SerializeField]
    private Button CameraButton;

    // Reference to Blink for Screenshot
    [SerializeField]
    private GameObject blink;

    // Reference to MaterialManager
    [SerializeField]
    public MaterialManager materialManager;

    // Reference to DrawingButton
    [SerializeField]
    public Button drawingButton;

    // DrawingButtonImage when mode off
    [SerializeField]
    public Sprite drawingButtonImage_Off;

    // DrawingButtonImage when mode on
    [SerializeField]
    public Sprite drawingButtonImage_On;



    void Start()
    {
        //Add a listener to the Material Button
        if (MaterialButton == null)
        {
            Logger.Instance.LogError("MaterialButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        MaterialButton.onClick.AddListener(MaterialButtonClickHandler);


        //Add a listener to the Camera Button
        CameraButton.onClick.AddListener(CameraButtonClickHandler);

        if (CameraButton == null)
        {
            Logger.Instance.LogError("CameraButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        //Add a listener to the Drawing Button
        drawingButton.onClick.AddListener(drawingButtonClickHandler);

        if (drawingButton == null)
        {
            Logger.Instance.LogError("drawingButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }
    }

    // Handler for clicks on Buttons

    void drawingButtonClickHandler()
    {
        if (drawingButton != null)
        {
            materialManager.drawingModeOn = !materialManager.drawingModeOn;
            Logger.Instance.LogInfo("Drawing Mode is On: " + materialManager.drawingModeOn);


            if (materialManager.drawingModeOn)
            {

                drawingButton.image.sprite = drawingButtonImage_On;
            }
            else
            {
                drawingButton.image.sprite = drawingButtonImage_Off;
            }

        }
    }

    void MaterialButtonClickHandler()

    {
        Logger.Instance.LogInfo("Material Button Clicked!");
        materialManager.materialChange();
    }

    void CameraButtonClickHandler()
    {
        Logger.Instance.LogInfo("Camera Button Clicked");
        CaptureScreenshot();

    }


    void CaptureScreenshot()
    {
        // Temporarily deactivate UI elements
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }

        // Create a filename based on the current timestamp
        string screenshotName = "UrbanARt_Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";

        // Capture the screenshot and save it in the project folder
        ScreenCapture.CaptureScreenshot(screenshotName);

        // Wait for a short moment
        // To give Unity time to capture the screenshot before reactivating UI elements
        StartCoroutine(ReactivateUIAfterDelay());

        
        if (blink != null)
        {
            StartCoroutine(BlinkCoroutine());
        }

        Logger.Instance.LogInfo("Screenshot aufgenommen: " + screenshotName);

    }

    // Reactivate UI Elements after Delay
    System.Collections.IEnumerator ReactivateUIAfterDelay()
    {
        // Wait a few frames (or time) before reactivating UI elements
        yield return null;


        // Reactivate UI elements
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = true;
        }
    }

    //Blink when take Screenshot
    private IEnumerator BlinkCoroutine()
    {
        blink.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        blink.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
