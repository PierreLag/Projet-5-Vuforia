using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class APIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static IEnumerator<List<Furniture>> GetAllFurnitures()
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

        yield return response;
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

        Coroutine spriteCoroutine = StartCoroutine(furniture.GetSprite());
        furnitureSO.preview = furniture.preview;

        return furnitureSO;
    }
}
