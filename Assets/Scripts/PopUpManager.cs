using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Script Name: PopUpManager.cs
 * Author: Tabea Spaenich
 * Date: November 25, 2023
 * Description: Manages the display and control of information popup windows.
 */

public class PopUpManager : MonoBehaviour
{
    // Button triggering the display of InfoPopUps.
    [SerializeField] private Button infoButton;

    // List of InfoPopUps 
    [SerializeField] private GameObject[] infoPopUps;

    // Refernce for the shown inforInfoPopUps
    private int infoPopUpPageCounter = 0;

    // Button triggering the display of the next InfoPopUp
    [SerializeField] private Button[] nextInfoPopUpPageButton;

    // Start is called before the first frame update
    void Start()
    {
        // Deactivate all InfoPopUp windows at the beginning
        foreach (var popUp in infoPopUps)
        {
            popUp.SetActive(false);
        }

        // Check if InfoPopUps Button is assigned
        if (infoButton == null)
        {
            Logger.Instance.LogError("InfoButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        // Add InfoButtonClickHandler to the InfoButton
        infoButton.onClick.AddListener(InfoButtonClickHandler);

        // Check if NextInfoPopUpPageButton is assigned
        if (nextInfoPopUpPageButton == null)
        {
            Logger.Instance.LogError("NextInfoPopUpPageButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        // Add NextInfoPopUpPageButtonClickHandler to all NextInfoPopUpPageButtons
        foreach (var button in nextInfoPopUpPageButton)
        {
            button.onClick.AddListener(NextInfoPopUpPageButtonClickHandler);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            TouchOutsideOfPopUp();
        }
    }

    // Close active Popup if touch is outside of PopupWindow
    void TouchOutsideOfPopUp()
    {
            // Check if the touch point is not over the popup object and if any InfoPopUps are open
            if (!IsTouchOverPopup(Input.GetTouch(0).position) && AreInfoPopUpsOpen())
            {
                // Call the method directly when a touch occurs outside the popup object
                ToggleInfoPopUps();
            }
    }

    // InfoButton Handler
    void InfoButtonClickHandler()
    {
        ToggleInfoPopUps();
    }

    // Show the next InfoPopUp
    void NextInfoPopUpPageButtonClickHandler()
    {
        Logger.Instance.LogInfo("Next Info Page Button Clicked!");

        infoPopUps[infoPopUpPageCounter].SetActive(false);
        infoPopUpPageCounter++;


        if (infoPopUpPageCounter < infoPopUps.Length)
        {
            infoPopUps[infoPopUpPageCounter].SetActive(true);
        }
    }

    // Close or Open InfoPopUp
    void ToggleInfoPopUps()
    {
        if (infoPopUps != null)
        {
            infoPopUps[infoPopUpPageCounter].SetActive(!infoPopUps[infoPopUpPageCounter].activeSelf);
            if (!infoPopUps[infoPopUpPageCounter].activeSelf)
            {
                infoPopUpPageCounter = 0;
            }
        }
    }

    // Return if InfoPopUp is touched
    bool IsTouchOverPopup(Vector2 touchPosition)
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Return if InfoPopUps are open
    public bool AreInfoPopUpsOpen()
    {
        foreach (var popUp in infoPopUps)
        {
            if (popUp.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
