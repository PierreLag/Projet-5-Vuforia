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
                                        + "Dimensions : " + furnitureDetails.width + "m x " + furnitureDetails.length + "m x " + furnitureDetails.height + "m" + '\n'
                                        + "Prix : " + furnitureDetails.price + " €");
    }

    /// <summary>
    /// Place cet objet en mode Mouvement.
    /// </summary>
    public void MoveFurniture()
    {
        customContentPositioningBehaviour.MovePlacedFurniture(gameObject);
    }

    /// <summary>
    /// Place cet objet en mode Rotation.
    /// </summary>
    public void RotateFurniture()
    {
        customContentPositioningBehaviour.RotatePlacedFurniture(gameObject);
    }

    /// <summary>
    /// Supprime cet objet.
    /// </summary>
    public void DeleteFurniture()
    {
        customContentPositioningBehaviour.DeletePlacedObject(gameObject);
    }

    /// <summary>
    /// Retourne les informations de ce mobilier.
    /// </summary>
    /// <returns>Les informations du mobilier.</returns>
    public FurnitureSO GetFurnitureSO()
    {
        return furnitureDetails;
    }
}
