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
    /// Place l'objet � l'index entr� dans la sc�ne.
    /// </summary>
    /// <param name="index">L'index de l'objet � placer, selon la liste objectsToPlace param�tr�e dans l'�diteur.</param>
    public void ShowTransparentFurniture(int index)
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[index]);
    }

    /// <summary>
    /// Retourne le mat�riau plein � appliquer dans les objets plac�s.
    /// </summary>
    /// <returns>Le mat�riau plein des objets � placer.</returns>
    public Material GetFullMaterial()
    {
        return fullMaterial;
    }

    /// <summary>
    /// Retourne le mat�riau transparent � appliquer dans les objets plac�s.
    /// </summary>
    /// <returns>Le mat�riau transparent des objets � placer.</returns>
    public Material GetTransparentMaterial()
    {
        return transparentMaterial;
    }
}
