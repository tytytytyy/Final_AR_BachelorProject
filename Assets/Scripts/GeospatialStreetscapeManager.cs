using System;
using System.Collections.Generic;
using System.IO;
using Google.XR.ARCoreExtensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


/*
 * Script Name: GeospatialStreetscapeManager.cs
 * Author: Tabea Spaenich
 * Date: December 05, 2023
 * Description: Manages geospatial streetscape elements in AR space,
 *              rendering buildings and terrains based on ARStreetscapeGeometry.
 */

// Options for Display of Terrain or Building Geometries
public class StreetscapeMenuOptions
{
    public bool BuildingsOn { get; set; }

    public bool TerrainsOn { get; set; }
}

public class GeospatialStreetscapeManager : MonoBehaviour
{
    // Provides ARCore Geospatial Streetscape Geometry APIs
    [SerializeField]
    private ARStreetscapeGeometryManager streetscapeGeometryManager;

    // Component to raycast against trackables
    [SerializeField]
    private ARRaycastManager raycastManager;

    // Reference to MaterialManager
    [SerializeField]
    public MaterialManager materialManager;

    // Reference to DrawingManager
    [SerializeField]
    private DrawingManager drawingManager;

    // All cached streetscape geometries
    private Dictionary<TrackableId, GameObject> streetscapeGeometryCached =
            new Dictionary<TrackableId, GameObject>();

    // List of all hits against streetscape geometries
    private static List<XRRaycastHit> hits = new List<XRRaycastHit>();

    // Reference of the options for displaying Terrain or Buildings
    private StreetscapeMenuOptions options = new StreetscapeMenuOptions();

    // Toggles display of Buildings
    [SerializeField]
    private UnityEngine.UI.Toggle buildingsToggle;

    // Toggles display of Terrain
    [SerializeField]
    private UnityEngine.UI.Toggle terrainsToggle;

    private void OnEnable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged += StreetscapeGeometriesChanged;

        options.BuildingsOn = buildingsToggle.isOn;
        options.TerrainsOn = terrainsToggle.isOn;

        buildingsToggle.onValueChanged.AddListener((_) =>
        {
            options.BuildingsOn = !options.BuildingsOn;
            if (!options.BuildingsOn)
            {
                DestroyAllRenderGeometry();
                Logger.Instance.LogInfo("Destroyed");

            }
        });

        terrainsToggle.onValueChanged.AddListener((_) =>
        {
            options.TerrainsOn = !options.TerrainsOn;
            if (!options.TerrainsOn)
            {

                DestroyAllRenderGeometry();
            }
        });

    }

    private void OnDisable()
    {
        streetscapeGeometryManager.StreetscapeGeometriesChanged -= StreetscapeGeometriesChanged;
    }


    private void StreetscapeGeometriesChanged(ARStreetscapeGeometriesChangedEventArgs geometries)
    {
        geometries.Added.ForEach(g => AddRenderGeometry(g));
        geometries.Updated.ForEach(g => UpdateRenderGeometry(g));
        geometries.Removed.ForEach(g => DestroyRenderGeometry(g));

    }

    // Add StreetScape geometries
    private void AddRenderGeometry(ARStreetscapeGeometry geometry)
    {

        if (!streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            if ((geometry.streetscapeGeometryType == StreetscapeGeometryType.Building && options.BuildingsOn)
                ||
               (geometry.streetscapeGeometryType == StreetscapeGeometryType.Terrain && options.TerrainsOn))
            {

                GameObject renderGeometryObject = new GameObject(
                    "StreetscapeGeometryMesh", typeof(MeshFilter), typeof(MeshRenderer));

                renderGeometryObject.GetComponent<MeshFilter>().mesh = geometry.mesh;

                renderGeometryObject.GetComponent<MeshRenderer>().material =
                 geometry.streetscapeGeometryType == StreetscapeGeometryType.Building ? materialManager.buildingMaterial : materialManager.terrainMaterial;

                renderGeometryObject.AddComponent<MeshCollider>();

                renderGeometryObject.transform.position = geometry.pose.position;
                renderGeometryObject.transform.rotation = geometry.pose.rotation;

                streetscapeGeometryCached.Add(geometry.trackableId, renderGeometryObject);

            }
        }

    }

    // Update StreetScape geometries
    private void UpdateRenderGeometry(ARStreetscapeGeometry geometry)
    {
        if (streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            GameObject renderGeometryObject = streetscapeGeometryCached[geometry.trackableId];

            renderGeometryObject.GetComponent<MeshRenderer>().material =
             geometry.streetscapeGeometryType == StreetscapeGeometryType.Building ? materialManager.buildingMaterial : materialManager.terrainMaterial;

            renderGeometryObject.transform.position = geometry.pose.position;
            renderGeometryObject.transform.rotation = geometry.pose.rotation;
        }
        else
        {
            // in case toggled off and on
            AddRenderGeometry(geometry);
        }
    }

    // Destroy StreetScape geometry
    private void DestroyRenderGeometry(ARStreetscapeGeometry geometry)
    {

        if (streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            var renderGeometryObject = streetscapeGeometryCached[geometry.trackableId];
            streetscapeGeometryCached.Remove(geometry.trackableId);
            Destroy(renderGeometryObject);
        }
    }

    // Add all StreetScape geometries
    private void DestroyAllRenderGeometry()
    {
        var keys = streetscapeGeometryCached.Keys;
        foreach (var key in keys)
        {
            var renderObject = streetscapeGeometryCached[key];
            Destroy(renderObject);
        }
        streetscapeGeometryCached.Clear();

    }


    private void Update()
    {

        if (materialManager.drawingModeOn)
        {
            drawing();
        }

    }

    // Draw on the Geometries
    public void drawing()
    {
        UnityEngine.Touch touch = Input.GetTouch(0);

        // make sure the screen is not touched and pointer is currently not over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //Check for Touch motion
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            //Get tap position 
            Vector2 screenTapPosition = touch.position;

            // Hit-Test against the Streetscape geometries
            if (raycastManager.RaycastStreetscapeGeometry(screenTapPosition, ref hits))
            {
                //Get the hit geometry
                ARStreetscapeGeometry streetscapegeometry =
                  streetscapeGeometryManager.GetStreetscapeGeometry(hits[0].trackableId);

                Logger.Instance.LogInfo(Convert.ToString(streetscapegeometry.streetscapeGeometryType));

                if (streetscapegeometry != null)
                {
                    // If Building was hit first
                    if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Building)
                    {
                        // If touch phase just started
                        if (touch.phase == UnityEngine.TouchPhase.Began)
                        {
                            var hitPose = hits[0].pose;

                            ARAnchor anchor = streetscapeGeometryManager.AttachAnchor(streetscapegeometry, hitPose);

                            drawingManager.CreateNewLine(hitPose, hitPose.position);

                        }

                        // If touch phase continues
                        else if (touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Stationary)
                        {

                            var hitPose = hits[0].pose;

                            Logger.Instance.LogInfo("Touchphase continues");
                            drawingManager.UpdateLine(hitPose.position);

                        }

                    }

                    // If Terrain was hit first
                    if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Terrain)
                    {

                        var hitPose = hits[0].pose;
                        drawingManager.DrawWithObjects(hitPose);

                    }


                }
            }


        }

    }

}

