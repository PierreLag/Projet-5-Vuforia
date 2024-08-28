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
    Camera aRCamera;
    [SerializeField]
    ARController arController;
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
        if (currentStatus != ARStatus.IDLE_MODE && finger.ScreenPosition.y >= leanTouchYThreshold) {
            initialDown = finger;
        }
    }

    private void OnFingerHeldDown(LeanFinger finger)
    {
        if (initialDown != null)
        {
            switch (currentStatus)
            {
                case ARStatus.NEW_OBJECT_MODE:
                    transparentObject.transform.Rotate(transparentObject.transform.up, -finger.GetDeltaDegrees(finger.StartScreenPosition));
                    break;
                case ARStatus.MOVEMENT_MODE:
                    float xtranslate = finger.ScreenPosition.x - finger.LastScreenPosition.x;
                    float ztranslate = finger.ScreenPosition.y - finger.LastScreenPosition.y;
                    activeObject.transform.Translate(xtranslate, 0, ztranslate);
                    break;
                case ARStatus.ROTATE_MODE:
                    activeObject.transform.Rotate(activeObject.transform.up, -finger.GetDeltaDegrees(finger.StartScreenPosition));
                    break;
                default:
                    break;
            }
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
                    ChangeStatus(ARStatus.IDLE_MODE);
                }
                break;
            case ARStatus.IDLE_MODE:
                break;
            default:
                ChangeStatus(ARStatus.IDLE_MODE);
                break;
        }
    }

    private void PlaceNewObject()
    {
        transparentObject.GetComponentInChildren<MeshRenderer>().material = arController.GetFullMaterial();
        transparentObject.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        placedObjects.Add(transparentObject);
        transparentObject = null;
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

        if (currentStatus == ARStatus.IDLE_MODE)
        {
            List<GameObject> furnitureUIs = new List<GameObject>();

            foreach (GameObject furniture in placedObjects)
            {
                furnitureUIs.Add(furniture.GetComponentInChildren<Canvas>().gameObject);
            }

            foreach (GameObject ui in furnitureUIs) { 
                ui.transform.LookAt(aRCamera.transform.position);
            }
        }
    }

    public void SetTransparentObject(GameObject newObject)
    {
        if (transparentObject != null)
            Destroy(transparentObject);

        transparentObject = Instantiate(newObject);
        transparentObject.SetActive(true);
        transparentObject.GetComponentInChildren<MeshRenderer>().material = arController.GetTransparentMaterial();
        transparentObject.GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        ChangeStatus(ARStatus.NEW_OBJECT_MODE);

        if (spawnedAnchorBehaviour != null)
            transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
    }

    public void MovePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.MOVEMENT_MODE);
    }

    public void RotatePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.ROTATE_MODE);
    }

    public void DeletePlacedObject(GameObject furniture)
    {
        placedObjects.Remove(furniture);
        Destroy(furniture);
    }

    private void ChangeStatus(ARStatus newStatus)
    {
        currentStatus = newStatus;

        List<GameObject> furnitureUIs = new List<GameObject>();

        foreach(GameObject furniture in placedObjects)
        {
            furnitureUIs.Add(furniture.GetComponentInChildren<Canvas>(true).gameObject);
        }

        if (newStatus == ARStatus.IDLE_MODE)
        {
            foreach (GameObject ui in furnitureUIs)
            {
                ui.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject ui in furnitureUIs)
            {
                ui.SetActive(false);
            }
        }
    }
}
