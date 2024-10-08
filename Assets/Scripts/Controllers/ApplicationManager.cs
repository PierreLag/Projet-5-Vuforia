using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _this;

    [SerializeField]
    CatalogueSO allFurnituresCatalogue;

    // Start is called before the first frame update
    void Awake()
    {
        if (_this != null)
            Destroy(this);
        else
        {
            _this = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Retourne la liste de tous les mobiliers disponibles dans le catalogue.
    /// </summary>
    /// <returns>Le catalogue de tous les mobiliers disponibles.</returns>
    public static CatalogueSO GetFurnitureList()
    {
        return _this.allFurnituresCatalogue;
    }

    /// <summary>
    /// Charge de mani�re additive la sc�ne de test de mobiliers en R�alit� Augment�e.
    /// </summary>
    public static void EnableARTestScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    /// <summary>
    /// D�charge la sc�ne de test de mobiliers en R�alit� Augment�e.
    /// </summary>
    public static void DisableARTestScene()
    {
        SceneManager.UnloadSceneAsync(1);
    }

    /// <summary>
    /// Ajoute tous les objets plac�s dans la sc�ne dans le panier de l'utilisateur, et d�charge la sc�ne.
    /// </summary>
    public static void GoToCartWithPlacedARItems()
    {
        List<GameObject> placedObjects = CustomContentPositioningBehaviour.GetPlacedObjects();
        List<FurnitureSO> placedFurniture = new List<FurnitureSO>();

        foreach (GameObject placedObject in placedObjects) {
            placedFurniture.Add(placedObject.GetComponent<FurnitureDisplayController>().GetFurnitureSO());
        }

        DisableARTestScene();
        NavigationUIController.FromARToCartWithItems(placedFurniture);
    }
}
