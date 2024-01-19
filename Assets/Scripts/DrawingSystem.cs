using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

/*
 * Script Name: DrawingSystem.cs
 * Author: Tabea Spaenich
 * Date: December 25, 2023
 * Description: Manages drawing lines in AR space using LineRenderer and ARAnchorManager.
 */

public class DrawingSystem : MonoBehaviour
{
    
    public static DrawingSystem Instance;

    private LineRenderer currentLine;
    private int positionCount = 2;

    [SerializeField]
    private ARAnchorManager anchorManager;

    [SerializeField]
    public Material lineMaterial;

    public void CreateNewLine(Vector3 position)
    {
        InitializeLineRenderer(position);

        currentLine.positionCount = positionCount;
        currentLine.SetPosition(0, position);//0
        Logger.Instance.LogInfo("Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(0));
        
        currentLine.SetPosition(1, position);  //1 Set the position at the incremented index
        Logger.Instance.LogInfo("Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(1));

        positionCount = positionCount + 1;
        currentLine.positionCount = positionCount;

        Logger.Instance.LogWarning("POSITION COUNT:"+Convert.ToString(positionCount));

    }

    public void UpdateLine(Vector3 position)
    {
        if (currentLine != null)
        {
            currentLine.SetPosition(positionCount-1, position);
            Logger.Instance.LogInfo("CONTINUE Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(1));

            positionCount = positionCount + 1;
            currentLine.positionCount = positionCount;

            currentLine.Simplify(1f);


        }
        else
        {
            Logger.Instance.LogError("no CURRENT LINE");

        }
    }

    public void EndLine()
    {
        //currentLine = null;
        positionCount = 2;
    }

    void InitializeLineRenderer(Vector3 position)
    {

        GameObject gameObject = new GameObject();
        if (gameObject == null)
            Logger.Instance.LogError("Error creating line ");
        else
        {
            Logger.Instance.LogInfo($"LineObjct created)");
        }


        // Füge eine LineRenderer-Komponente hinzu
        currentLine = gameObject.AddComponent<LineRenderer>();

        if (currentLine == null)
            Logger.Instance.LogError("Error creating line ");
        else
        {
            Logger.Instance.LogInfo($"LineRenderer created)");
        }


        // Setze die LineRenderer-Einstellungen
        currentLine.startWidth = 0.5f;
        currentLine.endWidth = 0.5f;
        currentLine.material = lineMaterial;
        currentLine.material.color = Color.red;
        currentLine.useWorldSpace = false;

    }


}
