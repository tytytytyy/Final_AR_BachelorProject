using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

/*
 * Script Name: DrawingManager.cs
 * Author: Tabea Spaenich
 * Date: December 25, 2023
 * Description: Manages drawing lines in AR space using LineRenderer and ARAnchorManager.
 */

public class DrawingManager : MonoBehaviour
{
    // Reference to the current LineRenderer component
    private LineRenderer currentLine;

    // Initial position count for the LineRenderer
    private int positionCount = 0;

    // Material for the line renderer
    [SerializeField]
    public Material lineMaterial;

    // Object that can be drawn into the Scene
    [SerializeField]
    private GameObject objectToSpawn;

    // Reference to ARAnchorManager
    [SerializeField]
    ARAnchorManager arAnchorManager;

    // Reference to Position of the last placed object
    private Vector3 PreviousPosition;

    // Minimum distance between objects
    private float minimumDistanceforObjects = 2f;


    // Create a new line
    public void CreateNewLine(Pose hitPose, Vector3 position)
    {

        // Create a new GameObject to represent the line

        GameObject gameObject = new GameObject();

        if (gameObject == null)
            Logger.Instance.LogError("Error creating line ");
        else
        {
            Logger.Instance.LogInfo($"LineObject created)");
        }

        // Add a LineRenderer component to the GameObject

        currentLine = gameObject.AddComponent<LineRenderer>();

        if (currentLine == null)
            Logger.Instance.LogError("Error creating line ");
        else
        {
            Logger.Instance.LogInfo($"LineRenderer created)");
        }

        arAnchorManager.AddAnchor(hitPose);

        // Set LineRenderer properties
        currentLine.startWidth = 0.4f;
        currentLine.endWidth = 0.4f;
        currentLine.material = lineMaterial;
        currentLine.material.color = Color.red;
        currentLine.useWorldSpace = true;

        positionCount = 0;

        // Set the initial position of the LineRenderer
        currentLine.SetPosition(positionCount, position);
        Logger.Instance.LogInfo("Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(0));

        positionCount++;

        currentLine.SetPosition(positionCount, position);
        Logger.Instance.LogInfo("Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(1));

    }

    // Update the line with a new position
    public void UpdateLine(Vector3 position)
    {
        if (currentLine != null)
        {
            positionCount++;
            currentLine.positionCount = positionCount;

            // Set the position at the incremented index
            currentLine.SetPosition(positionCount - 1, position);
            Logger.Instance.LogWarning("Position Pointn:" + Convert.ToString(currentLine.positionCount - 1) + "CONTINUE Hit point = " + position + " > Line Pos = " + currentLine.GetPosition(positionCount - 1));

            // Simplify the line to remove unnecessary vertices
            currentLine.Simplify(0.001f);

        }
        else
        {
            Logger.Instance.LogError("Error no current Line");

        }
    }

    //Draw with an Object 
    public void DrawWithObjects(Pose hitPose)
    {
        // Draw just if the Objects have a minimus distance to each other
        if (PreviousPosition != null && Mathf.Abs(Vector3.Distance(PreviousPosition, hitPose.position)) >= minimumDistanceforObjects)
        {

            Logger.Instance.LogInfo("Distance:"+ Convert.ToString(Mathf.Abs(Vector3.Distance(PreviousPosition, hitPose.position))));
            Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
            PreviousPosition = hitPose.position;
        }
        else if (PreviousPosition == null)
        {
            Instantiate(objectToSpawn, hitPose.position, hitPose.rotation);
            PreviousPosition = hitPose.position;

        }

    }
}