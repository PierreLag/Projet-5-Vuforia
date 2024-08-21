using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Threading.Tasks;

public class ARController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objectsToPlace;

    [SerializeField]
    Material fullMaterial;
    [SerializeField]
    Material transparentMaterial;

    [SerializeField]
    CustomContentPositioningBehaviour customContentPositioning;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject furniture in objectsToPlace)
        {
            furniture.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTransparentHokey()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[0]);
    }

    public void ShowTransparentCoffee()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[1]);
    }

    public void ShowTransparentChair()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[2]);
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
