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
            _this = this;
    }

    public CatalogueSO GetFurnitureList()
    {
        return allFurnituresCatalogue;
    }

    public void EnableARTestScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    public void DisableARTestScene()
    {
        SceneManager.UnloadSceneAsync(1);
    }
}
