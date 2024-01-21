using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;


/*
 * Script Name: MaterialManager.cs
 * Author: Tabea Spaenich
 * Date: December 10, 2023
 * Description: Manages the materials and components to change the materials
 */
public class MaterialManager : MonoBehaviour
{ 
    // Current Material of the buildings
    public Material buildingMaterial;

    // Material of the Terrain
    [SerializeField]
    public Material terrainMaterial;

    // Array of the materials for the buildings
    [SerializeField]
    public Material[] buildingMaterials;

    // Counter to choose material of buildings
    private int currentMaterialIndex = 0;

    // Drawing mode off or on
    public bool drawingModeOn = false;

    // VideoFileName for a material override
    string videoFileName = "trippy.mp4"; // Update with your video file name

    // Reference to the File Path for the material override
    string videoFilePath;

    // Videoplay to override material
    VideoPlayer videoPlayer;

    private void Start()
    {
        // Assign start material 
        buildingMaterial = buildingMaterials[currentMaterialIndex];
    }

    // Change building material
    public void materialChange()
    {

        // Increase the index and check whether it is outside the array.
        currentMaterialIndex = (currentMaterialIndex + 1) % buildingMaterials.Length;

        // Set the new material
        buildingMaterial = buildingMaterials[currentMaterialIndex];

        Logger.Instance.LogInfo(Convert.ToString("Changed Material to " + currentMaterialIndex + " name:" + buildingMaterials[currentMaterialIndex].name));
    }

    // Create a UV Map for the Mesh of the geometry
    private void CreateUVMap(GameObject renderGeometryObject)
    {

        Mesh mesh = renderGeometryObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].y, vertices[i].z);
        }
        renderGeometryObject.GetComponent<MeshFilter>().mesh.uv = uvs;

    }

    //Method to override Material with video
    private void ExchangeMaterialwithVideo(GameObject gameObject)
    {

        videoFilePath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url = videoFilePath;

        videoPlayer.isLooping = true;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.frame = 0;
        videoPlayer.targetCameraAlpha = 0.5F;
        videoPlayer.SetDirectAudioMute(1, true);

        videoPlayer.Play();

    }
}