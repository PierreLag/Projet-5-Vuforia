using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FurnitureDisplayController : MonoBehaviour
{
    CustomContentPositioningBehaviour customContentPositioningBehaviour;

    [SerializeField]
    private TextMeshProUGUI furnitureDescriptionBox;

    [SerializeField]
    private FurnitureSO furnitureDetails;

    // Start is called before the first frame update
    void Start()
    {
        customContentPositioningBehaviour = GameObject.Find("Plane Finder").GetComponent<CustomContentPositioningBehaviour>();

        furnitureDescriptionBox.SetText(furnitureDetails.name + '\n'
                                        + "Dimensions : " + furnitureDetails.width + " x " + furnitureDetails.length + " x " + furnitureDetails.height + '\n'
                                        + "Prix : " + furnitureDetails.price);
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
