using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IkeAR/FurnitureSO")]
public class FurnitureSO : ScriptableObject
{
    public new string name;
    public string description;
    public float width;
    public float length;
    public float height;
    public float price;
    public Sprite preview;
    public string category;

    public override string ToString()
    {
        return name + ", " + description + ", " + width + "m x " + length + "m x " + height + "m, " + price + "€, catégorie : " + category;
    }

    public void UpdateFromRuntime(FurnitureSO runtimeFurniture)
    {
        name = runtimeFurniture.name;
        description = runtimeFurniture.description;
        width = runtimeFurniture.width;
        length = runtimeFurniture.length;
        height = runtimeFurniture.height;
        price = runtimeFurniture.price;
        preview = runtimeFurniture.preview;
        category = runtimeFurniture.category;
    }
}
