using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class Furniture
{
    public string name;
    public string description;
    public float price;
    public float x;
    public float y;
    public float z;
    public string categoryName;
    public string imageReference;

    public Sprite preview;

    public static Furniture FromJSON(string json)
    {
        return JsonUtility.FromJson<Furniture>(json);
    }

    public static List<Furniture> ListFromJSON(string json)
    {
        Debug.Log("JSON input in ListFromJSON : " + json);
        JArray jfurnitures = JArray.Parse(json);
        List<Furniture> furnitures = jfurnitures.ToObject<List<Furniture>>();

        return furnitures;
    }

    public Vector3 GetDimensionsV3()
    {
        return new Vector3(x, y, z);
    }

    public IEnumerator GetSprite()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageReference);
        request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }

        Sprite sprite = null;
        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                break;
            case UnityWebRequest.Result.Success:
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                break;
            default:
                break;
        }

        preview = sprite;
    }
}
