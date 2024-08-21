using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CustomContentPositioningBehaviour : VuforiaMonoBehaviour
{
    AnchorBehaviour spawnedAnchorBehaviour;
    GameObject transparentObject;
    List<GameObject> placedObjects;

    [SerializeField]
    ARController arController;

    HitTestResult previousHitTestResult;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceNewObject(HitTestResult hitTestResult)
    {
        if (transparentObject != null)
        {
            transparentObject.GetComponent<MeshRenderer>().material = arController.GetFullMaterial();
            placedObjects.Add(transparentObject);
            transparentObject = null;
        }
    }

    public void MoveObjects(HitTestResult hitTestResult)
    {
        if (previousHitTestResult != null)
        {
            Vector3 deltaPosition = hitTestResult.Position - previousHitTestResult.Position;
            Quaternion deltaRotation = hitTestResult.Rotation * Quaternion.Inverse(previousHitTestResult.Rotation);

            if (transparentObject != null)
            {
                if (spawnedAnchorBehaviour == null)
                {
                    spawnedAnchorBehaviour = VuforiaBehaviour.Instance.ObserverFactory.CreateAnchorBehaviour("Placed object anchor", hitTestResult);
                    transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
                }
                transparentObject.transform.position = hitTestResult.Position;
                transparentObject.transform.rotation = hitTestResult.Rotation;
            }
        }

        previousHitTestResult = hitTestResult;
    }

    public void SetTransparentObject(GameObject newObject)
    {
        if (transparentObject != null)
            Destroy(transparentObject);

        transparentObject = Instantiate(newObject);
        transparentObject.SetActive(true);
        transparentObject.GetComponent<MeshRenderer>().material = arController.GetTransparentMaterial();

        if (spawnedAnchorBehaviour != null)
            transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
    }
}
