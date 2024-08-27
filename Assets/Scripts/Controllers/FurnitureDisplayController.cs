using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureDisplayController : MonoBehaviour
{
    CustomContentPositioningBehaviour customContentPositioningBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        customContentPositioningBehaviour = GameObject.Find("Plane Finder").GetComponent<CustomContentPositioningBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveFurniture()
    {
        customContentPositioningBehaviour.MovePlacedFurniture(gameObject);
    }

    public void RotateFurniture()
    {
        customContentPositioningBehaviour.RotatePlacedFurniture(gameObject);
    }

    public void DeleteFurniture()
    {
        customContentPositioningBehaviour.DeletePlacedObject(gameObject);
    }
}
