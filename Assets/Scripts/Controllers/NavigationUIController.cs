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
    VisualTreeAsset cartFurnitureDisplayTemplate;
    [SerializeField]
    Camera navigationCamera;

    static NavigationUIController _this;
    static UIDocument currentDocument;

    ApplicationManager manager;
    List<UIDocument> documentHistory;
    FurnitureSO displayedFurniture;
    Dictionary<FurnitureSO, int> userCart;

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
        userCart = new Dictionary<FurnitureSO, int>();

        VisualElement docRoot = currentDocument.rootVisualElement;
        docRoot.Q<Button>("ShopButton").clicked += GoToShop;
        docRoot.Q<Button>("ARTestButton").clicked += GoToARTest;
    }

    private void GoBack()
    {
        if (documentHistory.Count > 0)
        {
            ChangeNavigationDocument(documentHistory[documentHistory.Count - 1], true);
            documentHistory.RemoveAt(documentHistory.Count - 1);
        }
        else
        {
            Application.Quit();
        }
    }

    private void ChangeNavigationDocument(UIDocument newPage, bool isGoBack)
    {
        newPage.gameObject.SetActive(true);
        VisualElement newRoot = newPage.rootVisualElement;
        newRoot.Q<Button>("BackButton").clicked += GoBack;
        newRoot.Q<Button>("HomeButton").clicked += GoToHome;

        if (!isGoBack)
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
                    furnitureDisplay.Q<Label>(className: "FurniturePrice").text = furniture.price.ToString() + " �";

                    furnitureDisplay.clicked += () => GoToFurnitureDetails(furniture);

                    scrollView.contentViewport.Add(furnitureDisplay);
                }
                break;
            case "FurnitureDetailsDocument":
                newRoot.Q<VisualElement>("TitleContainer").Q<Label>().text = displayedFurniture.name;
                newRoot.Q<VisualElement>("Preview").style.backgroundImage = new StyleBackground(displayedFurniture.preview);
                newRoot.Q<Label>("Dimensions").text = "Dimensions : " + displayedFurniture.width + "m x " + displayedFurniture.length + "m x " + displayedFurniture.height + "m";
                newRoot.Q<Label>("Description").text = displayedFurniture.description;
                newRoot.Q<Label>("PriceTag").text = "Prix : " + displayedFurniture.price + " �";

                newRoot.Q<Button>("AddToCart").clicked += AddToCart;
                break;
            case "ShopCartDocument":
                DrawCartScrollView();
                break;
            default:
                break;
        }
    }

    private void GoToHome()
    {
        ChangeNavigationDocument(documentsReferences[0], false);
    }

    private void GoToShop()
    {
        ChangeNavigationDocument(documentsReferences[1], false);
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
        ChangeNavigationDocument(documentsReferences[2], false);
    }

    public static void ReenableNavigation()
    {
        currentDocument.gameObject.SetActive(true);
        _this.navigationCamera.gameObject.SetActive(true);
    }

    private void AddToCart()
    {
        if (userCart.ContainsKey(displayedFurniture))
        {
            userCart[displayedFurniture]++;
        }
        else
        {
            userCart.Add(displayedFurniture, 1);
        }
        ChangeNavigationDocument(documentsReferences[3], false);
    }

    private void IncrementFurnitureCart(FurnitureSO furniture)
    {
        userCart[furniture]++;
        DrawCartScrollView();
    }

    private void DecrementFurnitureCart(FurnitureSO furniture)
    {
        if (userCart[furniture] > 1)
        {
            userCart[furniture]--;
            DrawCartScrollView();
        }
    }

    private void RemoveFurnitureCart(FurnitureSO furniture)
    {
        userCart.Remove(furniture);
        DrawCartScrollView();
    }

    private void DrawCartScrollView()
    {
        ScrollView cartDisplay = currentDocument.rootVisualElement.Q<ScrollView>("Cart");

        cartDisplay.Clear();

        float totalPrice = 0f;

        foreach (KeyValuePair<FurnitureSO, int> item in userCart)
        {
            VisualElement cartFurniture = cartFurnitureDisplayTemplate.CloneTree().Q<VisualElement>(null, new string[] { "CartItem" });

            cartFurniture.Q<VisualElement>(className: "FurniturePreview").style.backgroundImage = new StyleBackground(item.Key.preview);
            cartFurniture.Q<Label>(className: "FurnitureName").text = item.Key.name;
            cartFurniture.Q<Label>(className: "Amount").text = item.Value.ToString();
            cartFurniture.Q<Label>(className: "Price").text = item.Key.price + " �";

            totalPrice += item.Key.price * item.Value;

            cartFurniture.Q<Button>(className: "ReduceQuantity").clicked += () => DecrementFurnitureCart(item.Key);
            cartFurniture.Q<Button>(className: "AugmentQuantity").clicked += () => IncrementFurnitureCart(item.Key);
            cartFurniture.Q<Button>(className: "Remove").clicked += () => RemoveFurnitureCart(item.Key);

            cartDisplay.Add(cartFurniture);
        }

        currentDocument.rootVisualElement.Q<Label>("TotalPrice").text = "Total : " + totalPrice + " �";
    }
}
