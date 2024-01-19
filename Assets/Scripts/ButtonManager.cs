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
 * Description: Manages the display and control of information popup windows.
 */
public class ButtonManager : MonoBehaviour
{

    [SerializeField]
    private Button MaterialButton;

    [SerializeField]
    private Button CameraButton;

    [SerializeField]
    private GameObject blink;

    [SerializeField]
    public MaterialManager materialManager;

    [SerializeField]
    public Button drawingButton;

    [SerializeField]
    public Sprite drawingButtonImage_Off;

    [SerializeField]
    public Sprite drawingButtonImage_On;



    // Start is called before the first frame update
    void Start()
    {
        //Add a listener to the Material Button
        if (MaterialButton == null)
        {
            Logger.Instance.LogError("MaterialButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        MaterialButton.onClick.AddListener(MaterialButtonClickHandler);

        CameraButton.onClick.AddListener(CameraButtonClickHandler);

        if (CameraButton == null)
        {
            Logger.Instance.LogError("CameraButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        drawingButton.onClick.AddListener(drawingButtonClickHandler);

        if (drawingButton == null)
        {
            Logger.Instance.LogError("drawingButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }
    }

    // Methods to handle button clicks

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

    void BuildingButtonClickHandler()
    {

    }


    void TerrainButtonClickHandler()
    {

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




    // Coroutine to change the image
    IEnumerator ChangeImageCoroutine()
    {
        // Change the image (replace 'myButton.image.overrideSprite' with the appropriate property in your case)

        Logger.Instance.LogInfo("Button changed!");

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        materialManager.materialChange();
        Logger.Instance.LogInfo("Start Material Change!");


        // Reset the image to its original state (optional)
        // Replace 'myButton.image.overrideSprite' with the appropriate property in your case
        MaterialButton.image.overrideSprite = null;
    }

    void CaptureScreenshot()
    {
        // Deaktiviere vorübergehend UI-Elemente (optional)
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }

        // Erstelle einen Dateinamen basierend auf dem aktuellen Zeitstempel
        string screenshotName = "UrbanARt_Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";

        // Erfasse den Screenshot und speichere ihn im Projektordner
        ScreenCapture.CaptureScreenshot(screenshotName);

        // Warte einen kurzen Moment (optional)
        // Damit Unity Zeit hat, den Screenshot zu erfassen, bevor UI-Elemente wieder aktiviert werden
        StartCoroutine(ReactivateUIAfterDelay());

        
        if (blink != null)
        {
            StartCoroutine(BlinkCoroutine());
        }

        // Optional: Debug-Ausgabe
        Logger.Instance.LogInfo("Screenshot aufgenommen: " + screenshotName);

    }



    System.Collections.IEnumerator ReactivateUIAfterDelay()
    {
        // Warte ein paar Frames (oder Zeit) bevor UI-Elemente wieder aktiviert werden
        yield return null;

        // Aktiviere UI-Elemente erneut (optional)
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = true;
        }
    }

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
