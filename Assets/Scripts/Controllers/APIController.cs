using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class APIController : MonoBehaviour
{
    private static object latestResponse;
    static APIController _this;

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

    public static IEnumerator GetAllFurnitures()
    {
        UnityWebRequest apiRequest = UnityWebRequest.Get("http://localhost/MYG/ikear/GetAllFurnitures.php");
        apiRequest.SendWebRequest();

        while(!apiRequest.isDone)
        {
            yield return null;
        }

        List<Furniture> response = new List<Furniture>();
        switch (apiRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                break;
            case UnityWebRequest.Result.Success:
                string furnituresJSON = apiRequest.downloadHandler.text;
                response = Furniture.ListFromJSON(furnituresJSON);
                break;
            default:
                break;
        }

        latestResponse = response;
    }

    public static void ResetResponse()
    {
        latestResponse = null;
    }

    public static object GetResponse()
    {
        return latestResponse;
    }

    public FurnitureSO ToScriptableObject(Furniture furniture)
    {
        FurnitureSO furnitureSO = new FurnitureSO();
        furnitureSO.name = furniture.name;
        furnitureSO.description = furniture.description;
        furnitureSO.length = furniture.x;
        furnitureSO.height = furniture.y;
        furnitureSO.width = furniture.z;
        furnitureSO.price = furniture.price;
        furnitureSO.category = furniture.categoryName;

        StartCoroutine(furniture.GetSprite());

        float timer = 0f;
        while (furniture.preview == null || timer <= 2f)
        {
            timer += Time.deltaTime;
        }
        furnitureSO.preview = furniture.preview;

        return furnitureSO;
    }
}
