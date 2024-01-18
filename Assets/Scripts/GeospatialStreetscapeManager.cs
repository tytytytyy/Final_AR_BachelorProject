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


public class StreetscapeMenuOptions
{
    public bool BuildingsOn { get; set; }

    public bool TerrainsOn { get; set; }
}

public class GeospatialStreetscapeManager : MonoBehaviour
{
    [SerializeField]
    private ARStreetscapeGeometryManager streetscapeGeometryManager;

    [SerializeField]
    private ARRaycastManager raycastManager;

    [SerializeField]
    public MaterialManager materialManager;

    [SerializeField]
    private GameObject objectToSpawn;

    [SerializeField]
    private GameObject objectToSpawn_Terrain;

    private Dictionary<TrackableId, GameObject> streetscapeGeometryCached =
            new Dictionary<TrackableId, GameObject>();

    private static List<XRRaycastHit> hits = new List<XRRaycastHit>();


    private StreetscapeMenuOptions options = new StreetscapeMenuOptions();

    [SerializeField]
    private UnityEngine.UI.Toggle buildingsToggle;

    [SerializeField]
    private UnityEngine.UI.Toggle terrainsToggle;


    //VideoPlayer Elements

    string videoFilePath;

    VideoPlayer videoPlayer;

    //Audio Elements


    [SerializeField]
    private UnityEngine.UI.Button materialButton;



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
            // in case we toggled it off and on
            AddRenderGeometry(geometry);
        }
    }

    private void DestroyRenderGeometry(ARStreetscapeGeometry geometry)
    {

        if (streetscapeGeometryCached.ContainsKey(geometry.trackableId))
        {
            var renderGeometryObject = streetscapeGeometryCached[geometry.trackableId];
            streetscapeGeometryCached.Remove(geometry.trackableId);
            Destroy(renderGeometryObject);
        }
    }

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

    public void drawing()
    {

        // make sure we're touching the screen and pointer is currently not over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //Check for Touch motion
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                //Get tap position 

                UnityEngine.Touch touch = Input.GetTouch(0);
                Vector2 screenTapPosition = touch.position;

                if (raycastManager.RaycastStreetscapeGeometry(screenTapPosition, ref hits))
                {

                    ARStreetscapeGeometry streetscapegeometry =
                      streetscapeGeometryManager.GetStreetscapeGeometry(hits[0].trackableId);

                    Logger.Instance.LogInfo(Convert.ToString(streetscapegeometry.streetscapeGeometryType));

                    if (streetscapegeometry != null)
                    {

                        if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Building)
                        {
                            var hitPose = hits[0].pose;

                            if (touch.phase == UnityEngine.TouchPhase.Began)
                            {

                            Logger.Instance.LogInfo("Touchphase Began");
                            DrawingSystem.Instance.CreateNewLine(hitPose);

                            }

                        else if (touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Stationary)
                            {

                            Logger.Instance.LogInfo("Touchphase continues");
                            DrawingSystem.Instance.UpdateLine(hitPose);

                        }
                        else if (touch.phase == UnityEngine.TouchPhase.Ended)
                        {
                            DrawingSystem.Instance.EndLine();
                        }

                    }

                        if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Terrain)
                        {
                            var hitPose = hits[0].pose;
                            Instantiate(objectToSpawn_Terrain, hitPose.position, hitPose.rotation);
                        }


                    }
                }


            }

        }
    

    private void drawObjects()
    {

        // make sure we're touching the screen and pointer is currently not over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;


        //Check for Touch motion
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            //Get tap position 
            Vector2 screenTapPosition = Input.GetTouch(0).position;

            if (raycastManager.RaycastStreetscapeGeometry(screenTapPosition, ref hits))
            {

                ARStreetscapeGeometry streetscapegeometry =
                  streetscapeGeometryManager.GetStreetscapeGeometry(hits[0].trackableId);

                Logger.Instance.LogInfo(Convert.ToString(streetscapegeometry.streetscapeGeometryType));


                if (streetscapegeometry != null)
                {

                    if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Building)
                    {
                        var hitPose = hits[0].pose;
                        Instantiate(objectToSpawn_Terrain, hitPose.position, hitPose.rotation);

                    }

                    if (streetscapegeometry.streetscapeGeometryType == StreetscapeGeometryType.Terrain)
                    {
                        var hitPose = hits[0].pose;
                        Instantiate(objectToSpawn_Terrain, hitPose.position, hitPose.rotation);
                    }

                }
            }


        }

    }

}