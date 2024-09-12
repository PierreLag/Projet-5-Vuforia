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
    /// M�thode customis�e activ�e au moment o� l'utilisateur appuie sur l'�cran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'�v�nement</param>
    private void OnFingerDown(LeanFinger finger)
    {
        if (currentStatus != ARStatus.IDLE_MODE && finger.ScreenPosition.y >= leanTouchYThreshold) {
            initialDown = finger;
        }
    }

    /// <summary>
    /// M�thode customis�e activ�e tant que l'utilisateur appuie sur l'�cran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'�v�nement</param>
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
    /// M�thode customis�e activ�e au moment o� l'utilisateur l�che le doigt de l'�cran.
    /// </summary>
    /// <param name="finger">Le doigt LeanFinger qui active l'�v�nement</param>
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
    /// Ancre l'objet transparent dans le plan, l'ajoute dans la liste d'objets plac�s, et lui donne le mat�riau plein.
    /// </summary>
    private void PlaceNewObject()
    {
        transparentObject.GetComponentInChildren<MeshRenderer>().material = arController.GetFullMaterial();
        transparentObject.GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
        placedObjects.Add(transparentObject);
        transparentObject = null;
    }

    /// <summary>
    /// Appel� � chaque Update de Vuforia, afin de d�placer l'objet transparent et de r�orienter les canvas sur chaque objet plac� vers la cam�ra.
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
    /// Instantie un nouveau objet transparent, et place la sc�ne en mode NEW_OBJECT_MODE. L'objet transparent suit le HitTest automatique de Vuforia.
    /// </summary>
    /// <param name="newObject">Le Nouvel objet � placer en transparence.</param>
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
    /// S�lectionne un objet � d�placer, et place l'application en mode MOVEMENT_MODE.
    /// </summary>
    /// <param name="furniture">L'objet plac� � d�placer.</param>
    public void MovePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.MOVEMENT_MODE);
    }

    /// <summary>
    /// S�lectionne un objet � tourner, et place l'application en mode ROTATE_MODE.
    /// </summary>
    /// <param name="furniture">L'objet plac� � tourner.</param>
    public void RotatePlacedFurniture(GameObject furniture)
    {
        activeObject = furniture;
        ChangeStatus(ARStatus.ROTATE_MODE);
    }

    /// <summary>
    /// S�lectionne un objet � supprimer.
    /// </summary>
    /// <param name="furniture">L'objet plac� � supprimer.</param>
    public void DeletePlacedObject(GameObject furniture)
    {
        placedObjects.Remove(furniture);
        Destroy(furniture);
    }

    /// <summary>
    /// Permet de changer le status de la sc�ne de R�alit� Augment�e, ce qui change les int�ractions de l'utilisateur avec l'�cran.
    /// </summary>
    /// <param name="newStatus">Le nouveau status de r�alit� augment�e. IDLE_MODE pour normal, NEW_OBJECT_MODE pour quand l'utilisateur compte placer un nouvel objet, MOVEMENT_MODE pour quand un objet plac� doit �tre d�plac�, et ROTATE_MODE pour quand un objet plac� doit �tre tourn�.</param>
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
    /// Retourne la liste d'objets plac�s dans la sc�ne.
    /// </summary>
    /// <returns></returns>
    public static List<GameObject> GetPlacedObjects()
    {
        return placedObjects;
    }
}
