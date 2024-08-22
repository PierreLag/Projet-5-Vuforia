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

    [SerializeField]
    ARController arController;
    [SerializeField]
    int leanTouchYThreshold;

    LeanFinger initialDown;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();

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
        if (finger.ScreenPosition.y >= leanTouchYThreshold)
        {
            Debug.Log("Finger down");
            initialDown = finger;
        }
    }

    private void OnFingerHeldDown(LeanFinger finger)
    {
        if (initialDown != null && transparentObject != null)
        {
            Debug.Log("Rotating item");
            transparentObject.transform.Rotate(transparentObject.transform.up, - finger.GetDeltaDegrees(finger.StartScreenPosition));
        }
    }

    private void OnFingerUp(LeanFinger finger)
    {
        if (initialDown != null)
        {
            Debug.Log("Placing item");
            PlaceNewObject();
            initialDown = null;
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
            spawnedAnchorBehaviour = VuforiaBehaviour.Instance.ObserverFactory.CreateAnchorBehaviour("Placed object anchor", hitTestResult);
            if (transparentObject != null)
                transparentObject.transform.parent = spawnedAnchorBehaviour.transform;
        }

        if (transparentObject != null && initialDown == null)
        {
            Debug.Log("Moving transparent object");
            transparentObject.transform.position = hitTestResult.Position;
            transparentObject.transform.rotation = hitTestResult.Rotation;
        }
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
