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

    public void ShowTransparentFurniture(int index)
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[index]);
    }

    public Material GetFullMaterial()
    {
        return fullMaterial;
    }

    public Material GetTransparentMaterial()
    {
        return transparentMaterial;
    }
}
