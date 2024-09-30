using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _this;

    [SerializeField]
    CatalogueSO allFurnituresCatalogue;
    [SerializeField]
    APIController apiController;

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
    /// Charge de manière additive la scène de test de mobiliers en Réalité Augmentée.
    /// </summary>
    public static void EnableARTestScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Décharge la scène de test de mobiliers en Réalité Augmentée.
    /// </summary>
    public static void DisableARTestScene()
    {
        SceneManager.UnloadSceneAsync(1);
    }

    /// <summary>
    /// Ajoute tous les objets placés dans la scène dans le panier de l'utilisateur, et décharge la scène.
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

    public static async Task UpdateAllFurnitures()
    {
        _this.StartCoroutine(APIController.GetAllFurnitures());

        int timesWaiting = 0;
        object response = null;
        while (response == null && timesWaiting <= 4)
        {
            await Task.Delay(100);
            response = APIController.GetResponse();
            timesWaiting++;
        }
        Debug.Log("Is Response null ? " + (response == null ? "Yes" : "No"));

        List<Furniture> furnitures = (List<Furniture>)response;
        APIController.ResetResponse();

        if (furnitures != null)
        {
            /*_this.allFurnituresCatalogue.furnitures.Clear();
            foreach (Furniture furniture in furnitures)
            {
                Task<FurnitureSO> castingTask = _this.apiController.ToScriptableObject(furniture);
                await castingTask;
                _this.allFurnituresCatalogue.furnitures.Add((FurnitureSO)castingTask.Result);
                Debug.Log(castingTask.Result);
            }*/
            int i = 0;
            for (i = 0; i < _this.allFurnituresCatalogue.furnitures.Count && i < furnitures.Count; i++)
            {
                Task<FurnitureSO> castingTask = _this.apiController.ToScriptableObject(furnitures[i]);
                await castingTask;
                _this.allFurnituresCatalogue.furnitures[i].UpdateFromRuntime(castingTask.Result);
            }
            for (i = i; i < furnitures.Count; i++)
            {
                Task<FurnitureSO> castingTask = _this.apiController.ToScriptableObject(furnitures[i]);
                await castingTask;
                _this.allFurnituresCatalogue.furnitures.Add(castingTask.Result);
            }
        }
    }
}
