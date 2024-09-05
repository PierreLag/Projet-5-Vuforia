using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationUIController : MonoBehaviour
{
    [SerializeField]
    UIDocument[] documentsReferences;
    [SerializeField]
    VisualTreeAsset furnitureNavigationTemplate;

    UIDocument currentDocument;
    ApplicationManager manager;

    // Start is called before the first frame update
    void Start()
    {
        currentDocument = documentsReferences[0];
        manager = GameObject.FindObjectOfType<ApplicationManager>();

        currentDocument.rootVisualElement.Q<Button>("ShopButton").clicked += GoToShop;
    }

    private void GoBack()
    {
        UIDocument previousDoc = currentDocument;
        GameObject.FindObjectOfType<UIDocument>().gameObject.SetActive(false);
        previousDoc.gameObject.SetActive(true);
    }

    private void ChangeNavigationDocument(UIDocument newPage)
    {
        VisualElement newRoot = newPage.rootVisualElement;
        newRoot.Q<Button>("BackButton").clicked += GoBack;

        newPage.gameObject.SetActive(true);
        currentDocument.gameObject.SetActive(false);
        currentDocument = newPage;

        switch (newPage.gameObject.name)
        {
            case "HomeDocument":

                break;
            case "FurnitureShopDocument":
                CatalogueSO allFurnitures = manager.GetFurnitureList();
                ScrollView scrollView = newRoot.Q<ScrollView>("FurnitureSelection");

                foreach (FurnitureSO furniture in allFurnitures.furnitures)
                {
                    VisualElement furnitureDisplay = furnitureNavigationTemplate.CloneTree().Q<VisualElement>(null, new string[] { "FurnitureDisplay" });

                    furnitureDisplay.Q<Label>(className: "FurnitureName").text = furniture.name;
                    furnitureDisplay.Q<Label>(className: "FurnitureDescription").text = furniture.description;
                    furnitureDisplay.Q<Label>(className: "FurniturePrice").text = furniture.price.ToString();

                    scrollView.Add(furnitureDisplay);
                }
                break;
            default:
                break;
        }
    }

    private void GoToShop()
    {
        ChangeNavigationDocument(documentsReferences[1]);
    }
}
