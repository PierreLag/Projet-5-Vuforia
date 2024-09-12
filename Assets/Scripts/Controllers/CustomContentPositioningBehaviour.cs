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

    /// <summary>
    /// Méthode customisée activée au moment où l'utilisateur appuie sur l'écran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'événement</param>
    private void OnFingerDown(LeanFinger finger)
    {
        if (currentStatus != ARStatus.IDLE_MODE && finger.ScreenPosition.y >= leanTouchYThreshold) {
            initialDown = finger;
        }
    }

    /// <summary>
    /// Méthode customisée activée tant que l'utilisateur appuie sur l'écran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'événement</param>
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

    /// <summary>
    /// Méthode customisée activée au moment où l'utilisateur lâche le doigt de l'écran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'événement</param>
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

    /// <summary>
    /// Ancre l'objet transparent dans le plan, l'ajoute dans la liste d'objets placés, et lui donne le matériau plein.
    /// </summary>
    private void PlaceNewObject()
    {
        transparentObject.GetComponentInChildren<MeshRenderer>().material = arController.GetFullMaterial();
        transparentObject.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        placedObjects.Add(transparentObject);
        transparentObject = null;
    }

    /// <summary>
    /// Appelé à chaque Update de Vuforia, afin de déplacer l'objet transparent et de réorienter les canvas sur chaque objet placé vers la caméra.
    /// </summary>
    /// <param name="hitTestResult">Performed hit test in the world.</param>
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

    /// <summary>
    /// Instantie un nouveau objet transparent, et place la scène en mode NEW_OBJECT_MODE. L'objet transparent suit le HitTest automatique de Vuforia.
    /// </summary>
    /// <param name="newObject">Le Nouvel objet à placer en transparence.</param>
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

    /// <summary>
    /// Sélectionne un objet à déplacer, et place l'application en mode MOVEMENT_MODE.
    /// </summary>
    /// <param name="furniture">L'objet placé à déplacer.</param>
    public void MovePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.MOVEMENT_MODE);
    }

    /// <summary>
    /// Sélectionne un objet à tourner, et place l'application en mode ROTATE_MODE.
    /// </summary>
    /// <param name="furniture">L'objet placé à tourner.</param>
    public void RotatePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.ROTATE_MODE);
    }

    /// <summary>
    /// Sélectionne un objet à supprimer.
    /// </summary>
    /// <param name="furniture">L'objet placé à supprimer.</param>
    public void DeletePlacedObject(GameObject furniture)
    {
        placedObjects.Remove(furniture);
        Destroy(furniture);
    }

    /// <summary>
    /// Permet de changer le status de la scène de Réalité Augmentée, ce qui change les intéractions de l'utilisateur avec l'écran.
    /// </summary>
    /// <param name="newStatus">Le nouveau status de réalité augmentée. IDLE_MODE pour normal, NEW_OBJECT_MODE pour quand l'utilisateur compte placer un nouvel objet, MOVEMENT_MODE pour quand un objet placé doit être déplacé, et ROTATE_MODE pour quand un objet placé doit être tourné.</param>
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

    /// <summary>
    /// Retourne la liste d'objets placés dans la scène.
    /// </summary>
    /// <returns></returns>
    public static List<GameObject> GetPlacedObjects()
    {
        return placedObjects;
    }
}
