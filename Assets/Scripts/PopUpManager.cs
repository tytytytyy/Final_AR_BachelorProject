using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{

    [SerializeField]
    private Button InfoButton;

    [SerializeField]
    public GameObject[] InfoPopUps;

    private int InfoPopUpPageCounter = 0;

    [SerializeField]
    private Button[] NextInfoPopUpPageButton;



    // Start is called before the first frame update
    void Start()
    {

        //Deactivate all InfoPopUp windows
        for (int i = 0; i < InfoPopUps.Length; i++)
        {
            InfoPopUps[i].SetActive(false);

        }

        if (InfoButton == null)
        {
            Logger.Instance.LogError("InfoButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        InfoButton.onClick.AddListener(InfoButtonClickHandler);

        if (NextInfoPopUpPageButton == null)
        {
            Logger.Instance.LogError("NextInfoPopUpPageButton component not assigned. Please assign the Button in the Inspector.");
            return;
        }

        for (int i = 0; i < NextInfoPopUpPageButton.Length; i++)
        {
            NextInfoPopUpPageButton[i].onClick.AddListener(NextInfoPopUpPageButtonClickHandler);

        }

    }

    // Update is called once per frame
    void Update()
    {

        //Close active PopUp if touch outside of PopUpWindow
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            // Check if the first touch command has started
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Check if the touch point is not over the pop-up object
                if (!IsTouchOverPopup(Input.GetTouch(0).position) && AreInfoPopUpsOpen())

                {
                    // Call the method directly when a touch occurs outside the pop-up object
                    ToogleInfoPopUps();
                }
            }
        }
    }

    //Button Handlers

    void InfoButtonClickHandler()
    {

        ToogleInfoPopUps();

    }

    void NextInfoPopUpPageButtonClickHandler()
    {
        Logger.Instance.LogInfo("Next Info Page Button Clicked!");

        InfoPopUps[InfoPopUpPageCounter].SetActive(false);
        InfoPopUpPageCounter++;
        if (InfoPopUpPageCounter <= InfoPopUps.Length)
        {
            InfoPopUps[InfoPopUpPageCounter].SetActive(true);
        }

    }

    //Close or Open Info Pop Up 
    void ToogleInfoPopUps()
    {
        if (InfoPopUps != null)
        {
            InfoPopUps[InfoPopUpPageCounter].SetActive(!InfoPopUps[InfoPopUpPageCounter].activeSelf);
            if (InfoPopUps[InfoPopUpPageCounter].activeSelf == false)
            {
                InfoPopUpPageCounter = 0;
            }
        }

    }

    bool IsTouchOverPopup(Vector2 touchPosition)
    {

        return EventSystem.current.IsPointerOverGameObject();

    }

    // Return if InfoPopUps are open
    public bool AreInfoPopUpsOpen() { 
    

        for (int i = 0; i<InfoPopUps.Length; i++)
        {
           if(InfoPopUps[i].activeSelf)
            {
                return true;
            }

        }
        return false;
        
    }

}
