using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Threading.Tasks;

public class ARController : MonoBehaviour
{
    [SerializeField]
    Material fullMaterial;
    [SerializeField]
    Material transparentMaterial;

    [SerializeField]
    CustomContentPositioningBehaviour customContentPositioning;
    [SerializeField]
    List<GameObject> objectsToPlace;

    //ApplicationManager manager;

    // Start is called before the first frame update
    void Start()
    {
        //manager = GameObject.FindObjectOfType<ApplicationManager>();

        foreach (GameObject furniture in objectsToPlace)
        {
            furniture.SetActive(false);
        }
    }

    /// <summary>
    /// Place l'objet à l'index entré dans la scène.
    /// </summary>
    /// <param name="index">L'index de l'objet à placer, selon la liste objectsToPlace paramétrée dans l'éditeur.</param>
    public void ShowTransparentFurniture(int index)
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[index]);
    }

    /// <summary>
    /// Retourne le matériau plein à appliquer dans les objets placés.
    /// </summary>
    /// <returns>Le matériau plein des objets à placer.</returns>
    public Material GetFullMaterial()
    {
        return fullMaterial;
    }

    /// <summary>
    /// Retourne le matériau transparent à appliquer dans les objets placés.
    /// </summary>
    /// <returns>Le matériau transparent des objets à placer.</returns>
    public Material GetTransparentMaterial()
    {
        return transparentMaterial;
    }
}
