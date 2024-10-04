using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

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

    public async Task<FurnitureSO> ToScriptableObject(Furniture furniture)
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

        int timesWaiting = 0;
        while (furniture.preview == null && timesWaiting <= 4)
        {
            await Task.Delay(100);
            timesWaiting++;
        }
        furnitureSO.preview = furniture.preview;

        return furnitureSO;
    }

    public static IEnumerator Login(string username, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("login", username));
        formData.Add(new MultipartFormDataSection("password", password));

        UnityWebRequest apiRequest = UnityWebRequest.Post("http://localhost/MYG/ikear/Login.php", formData);
        yield return apiRequest.SendWebRequest();

        switch (apiRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                latestResponse = "Connection error";
                break;
            case UnityWebRequest.Result.Success:
                string userJSON = apiRequest.downloadHandler.text;
                Debug.Log("Login JSON : " + userJSON);
                if (userJSON == "")
                {
                    latestResponse = "Login mismatch";
                } else
                {
                    ApplicationManager.SetUser(User.FromJSON(userJSON));
                    latestResponse = "Success";
                }
                break;
            default:
                break;
        }
    }

    public static IEnumerator CreateAccount(string username, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("login", username));
        formData.Add(new MultipartFormDataSection("password", password));
        Debug.Log("Post form prepared for sending");

        UnityWebRequest apiRequest = UnityWebRequest.Post("http://localhost/MYG/ikear/AddUser.php", formData);
        yield return apiRequest.SendWebRequest();
        Debug.Log("Request ongoing");

        if (apiRequest.downloadHandler.text.Contains("Error :"))
        {
            if (apiRequest.downloadHandler.text == "Error : Connection to server failed.")
            {
                latestResponse = "Connection error";
            }
            if (apiRequest.downloadHandler.text == "Error : User already exists.")
            {
                latestResponse = "Already existing user";
            }
        }
        else
        {
            ApplicationManager.SetUser(new User(username));
            latestResponse = "Success";
        }

        latestResponse = apiRequest.result;
    }
}
