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
    [SerializeField]
    Camera navigationCamera;

    static NavigationUIController _this;
    static UIDocument currentDocument;

    ApplicationManager manager;
    List<UIDocument> documentHistory;
    FurnitureSO displayedFurniture;

    private void Awake()
    {
        if (_this != null)
            Destroy(this);
        else
            _this = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentDocument = documentsReferences[0];
        manager = GameObject.FindObjectOfType<ApplicationManager>();
        documentHistory = new List<UIDocument>();
        documentHistory.Add(currentDocument);

        VisualElement docRoot = currentDocument.rootVisualElement;
        docRoot.Q<Button>("ShopButton").clicked += GoToShop;
        docRoot.Q<Button>("ARTestButton").clicked += GoToARTest;
    }

    private void GoBack()
    {
        if (documentHistory.Count > 1)
        {
            ChangeNavigationDocument(documentHistory[documentHistory.Count - 1]);
            documentHistory.RemoveAt(documentHistory.Count - 1);
        }
        else
        {
            Application.Quit();
        }
    }

    private void ChangeNavigationDocument(UIDocument newPage)
    {
        newPage.gameObject.SetActive(true);
        VisualElement newRoot = newPage.rootVisualElement;
        newRoot.Q<Button>("BackButton").clicked += GoBack;

        documentHistory.Add(currentDocument);
        currentDocument.gameObject.SetActive(false);
        currentDocument = newPage;

        switch (newPage.gameObject.name)
        {
            case "HomeDocument":
                newRoot.Q<Button>("ShopButton").clicked += GoToShop;
                newRoot.Q<Button>("ARTestButton").clicked += GoToARTest;
                break;
            case "FurnitureShopDocument":
                CatalogueSO allFurnitures = ApplicationManager.GetFurnitureList();
                ScrollView scrollView = newRoot.Q<ScrollView>("FurnitureSelection");

                foreach (FurnitureSO furniture in allFurnitures.furnitures)
                {
                    Button furnitureDisplay = furnitureNavigationTemplate.CloneTree().Q<Button>(null, new string[] { "FurnitureDisplay" });

                    furnitureDisplay.Q<Label>(className: "FurnitureName").text = furniture.name;
                    furnitureDisplay.Q<Label>(className: "FurnitureDescription").text = furniture.description;
                    furnitureDisplay.Q<Label>(className: "FurniturePrice").text = furniture.price.ToString() + " €";

                    furnitureDisplay.clicked += () => GoToFurnitureDetails(furniture);

                    scrollView.contentViewport.Add(furnitureDisplay);
                }
                break;
            case "FurnitureDetailsDocument":
                newRoot.Q<VisualElement>("TitleContainer").Q<Label>().text = displayedFurniture.name;
                newRoot.Q<VisualElement>("Preview").style.backgroundImage = new StyleBackground(displayedFurniture.preview);
                newRoot.Q<Label>("Dimensions").text = "Dimensions : " + displayedFurniture.width + " x " + displayedFurniture.length + " x " + displayedFurniture.height;
                newRoot.Q<Label>("Description").text = displayedFurniture.description;
                newRoot.Q<Label>("PriceTag").text = "Prix : " + displayedFurniture.price + " €";
                break;
            default:
                break;
        }
    }

    private void GoToShop()
    {
        ChangeNavigationDocument(documentsReferences[1]);
    }

    private void GoToARTest()
    {
        ApplicationManager.EnableARTestScene();
        currentDocument.gameObject.SetActive(false);
        navigationCamera.gameObject.SetActive(false);
    }

    private void GoToFurnitureDetails(FurnitureSO furniture)
    {
        displayedFurniture = furniture;
        ChangeNavigationDocument(documentsReferences[2]);
    }

    public static void ReenableNavigation()
    {
        currentDocument.gameObject.SetActive(true);
        _this.navigationCamera.gameObject.SetActive(true);
    }
}
