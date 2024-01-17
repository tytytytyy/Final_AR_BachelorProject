using System;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{ 
    public Material buildingMaterial;

    [SerializeField]
    public Material terrainMaterial;

    public Material[] buildingMaterials;

    private int currentIndex = 0;

    public Boolean drawingModeOn = false;

    private void Start()
    {
        // Start Material zuweisen
        buildingMaterial = buildingMaterials[currentIndex];
    }

    public void materialChange()
    {

        // Erh�he den Index und �berpr�fe, ob er au�erhalb des Arrays ist.
        currentIndex = (currentIndex + 1) % buildingMaterials.Length;

        // Setze das neue Material.
        buildingMaterial = buildingMaterials[currentIndex];

        Logger.Instance.LogInfo(Convert.ToString("Changed Material to " + currentIndex + " name:" + buildingMaterials[currentIndex].name));
    }

}