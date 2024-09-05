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
    List<FurnitureDisplayController> objectsToPlace;

    ApplicationManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<ApplicationManager>();

        foreach (FurnitureDisplayController furniture in objectsToPlace)
        {
            furniture.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTransparentHokey()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[0].gameObject);
    }

    public void ShowTransparentCoffee()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[1].gameObject);
    }

    public void ShowTransparentChair()
    {
        customContentPositioning.SetTransparentObject(objectsToPlace[2].gameObject);
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
