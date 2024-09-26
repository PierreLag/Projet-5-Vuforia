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
}
