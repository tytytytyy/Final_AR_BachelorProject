using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DrawingSystem : MonoBehaviour
{

    public static DrawingSystem Instance;

    private LineRenderer currentLine;
    private int positionCount = 0;

    [SerializeField]
    private ARAnchorManager anchorManager;

    [SerializeField]
    public Material lineMaterial;

    void Awake()
    {
        Instance = this;
    }

    public void CreateNewLine(Pose hitPose)
    {
 
        InitializeLineRenderer();   
        currentLine.SetPosition(positionCount, hitPose.position);

        ARAnchor anchor = anchorManager.AddAnchor(new Pose(hitPose.position, hitPose.rotation));

        if (anchor == null)
            Logger.Instance.LogError("Error creating reference point");
        else
        {
            Logger.Instance.LogInfo($"Anchor created)");
        }

        Logger.Instance.LogInfo(Convert.ToString(positionCount));
    }

    public void UpdateLine(Pose hitPose)
    {
        if (currentLine != null)
        {
            positionCount = positionCount++;
            currentLine.positionCount = positionCount;
            currentLine.SetPosition(positionCount, hitPose.position);
            Logger.Instance.LogInfo(Convert.ToString(positionCount));

            currentLine.Simplify(0.1f);

        }
    }

    public void EndLine()
    {
        currentLine = null;
    }

    void InitializeLineRenderer()
    {

        GameObject lineObject = new GameObject("Line");

        // Füge eine LineRenderer-Komponente hinzu
        currentLine = gameObject.AddComponent<LineRenderer>();

        // Setze die LineRenderer-Einstellungen
        currentLine.startWidth = 0.05f;
        currentLine.endWidth = 0.05f;
        currentLine.material = lineMaterial; // Verwende das zugewiesene Material
        currentLine.positionCount = 1; // Keine Punkte zu Beginn
    }


}
