using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Lean.Touch;
using System.Threading.Tasks;

public class CustomContentPositioningBehaviour : VuforiaMonoBehaviour
{
    static List<GameObject> placedObjects;

    AnchorBehaviour spawnedAnchorBehaviour;
    GameObject transparentObject;
    GameObject activeObject;

    [SerializeField]
    Camera aRCamera;
    [SerializeField]
    ARController arController;
    [SerializeField]
    int leanTouchYThreshold;
    [SerializeField][Range(0f, 1f)]
    float objectMovementSensitivity;

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
                    float xFingerMovement = finger.ScaledDelta.x * objectMovementSensitivity;
                    float yFingerMovement = finger.ScaledDelta.y * objectMovementSensitivity;

                    Transform relativeTransform = aRCamera.transform;
                    relativeTransform.LookAt(new Vector3(relativeTransform.position.x + relativeTransform.forward.x,
                                                         relativeTransform.position.y,
                                                         relativeTransform.position.z + relativeTransform.forward.z));

                    activeObject.transform.Translate(xFingerMovement, 0, yFingerMovement, relativeTransform);
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
                activeObject = null;
                initialDown = null;
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
                furnitureUIs.Add(furniture.GetComponentInChildren<Canvas>().transform.parent.gameObject);
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

    private async void ChangeStatus(ARStatus newStatus)
    {
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
                await Task.Delay(100);
            }
        }

        currentStatus = newStatus;
    }

    public static List<GameObject> GetPlacedObjects()
    {
        return placedObjects;
    }
}
