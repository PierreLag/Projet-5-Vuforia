using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Lean.Touch;

public class CustomContentPositioningBehaviour : VuforiaMonoBehaviour
{
    AnchorBehaviour spawnedAnchorBehaviour;
    GameObject transparentObject;
    List<GameObject> placedObjects;
    GameObject activeObject;

    [SerializeField]
    ARController arController;
    [SerializeField]
    Canvas newFurnitureCanvas;
    [SerializeField]
    int leanTouchYThreshold;

    LeanFinger initialDown;
    ARStatus currentStatus;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();

        currentStatus = ARStatus.IDLE_MODE;

        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerHeldDown;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnFingerDown(LeanFinger finger)
    {
        switch (currentStatus) {
            case ARStatus.NEW_OBJECT_MODE:
                if (finger.ScreenPosition.y >= leanTouchYThreshold)
                {
                    initialDown = finger;
                }
                break;
            default:
                break;
        }
    }

    private void OnFingerHeldDown(LeanFinger finger)
    {
        switch (currentStatus) {
            case ARStatus.NEW_OBJECT_MODE:
                if (initialDown != null && transparentObject != null)
                {
                    transparentObject.transform.Rotate(transparentObject.transform.up, -finger.GetDeltaDegrees(finger.StartScreenPosition));
                }
                break;
            default:
                break;
        }
    }

    private void OnFingerUp(LeanFinger finger)
    {
        switch (currentStatus) {
            case ARStatus.NEW_OBJECT_MODE:
                if (initialDown != null)
                {
                    PlaceNewObject();
                    initialDown = null;
                }
                currentStatus = ARStatus.IDLE_MODE;
                break;
            default:
                break;
        }
    }

    private void PlaceNewObject()
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
        if (spawnedAnchorBehaviour == null)
        {
            spawnedAnchorBehaviour = VuforiaBehaviour.Instance.ObserverFactory.CreateAnchorBehaviour("Placed objects anchor", hitTestResult);
            if (transparentObject != null)
                transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
        }

        if (transparentObject != null && initialDown == null)
        {
            transparentObject.transform.position = hitTestResult.Position;
            transparentObject.transform.rotation = hitTestResult.Rotation;
        }
    }

    public void SetTransparentObject(GameObject newObject)
    {
        currentStatus = ARStatus.NEW_OBJECT_MODE;

        if (transparentObject != null)
            Destroy(transparentObject);

        transparentObject = Instantiate(newObject);
        transparentObject.SetActive(true);
        transparentObject.GetComponent<MeshRenderer>().material = arController.GetTransparentMaterial();

        if (spawnedAnchorBehaviour != null)
            transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
    }

    public void MovePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        currentStatus = ARStatus.MOVEMENT_MODE;
    }

    private void ChangeStatus(ARStatus newStatus)
    {
        currentStatus = newStatus;

        GameObject[] furnitureUIs = GameObject.FindGameObjectsWithTag("World UI");

        switch (newStatus)
        {
            case ARStatus.IDLE_MODE:
                break;
            case ARStatus.NEW_OBJECT_MODE:
                break;
            case ARStatus.MOVEMENT_MODE:
                break;
            case ARStatus.ROTATE_MODE:
                break;
        }
    }
}
