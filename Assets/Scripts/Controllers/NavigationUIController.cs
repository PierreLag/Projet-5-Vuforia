using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NavigationUIController : MonoBehaviour
{
    [SerializeField]
    List<UIDocument> documentsReferences;
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
        {
            _this = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentDocument = documentsReferences[0];
        manager = GameObject.FindObjectOfType<ApplicationManager>();
        documentHistory = new List<UIDocument>();
        userCart = new Dictionary<FurnitureSO, int>();

        VisualElement docRoot = currentDocument.rootVisualElement;
        docRoot.Q<Button>("BackButton").clicked += GoBack;
        docRoot.Q<Button>("QuickMenuButton").clicked += ShowQuickNavigation;
        docRoot.Q<Button>("ShopButton").clicked += GoToShop;
        docRoot.Q<Button>("ARTestButton").clicked += GoToARTest;
    }

    /// <summary>
    /// Cette méthode permet de revenir en arrière dans l'historique de navigation de l'utilisateur. Si l'historique est vide, quitte l'application à la place.
    /// </summary>
    private void GoBack()
    {
        if (documentHistory.Count > 0)
        {
            ChangeNavigationDocument(documentHistory[documentHistory.Count - 1], false);
            documentHistory.RemoveAt(documentHistory.Count - 1);
        }
        else
        {
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #else
            Application.Quit();
            #endif
        }
    }

    /// <summary>
    /// Cette méthode permet de changer la page de navigation vers un autre document.
    /// </summary>
    /// <param name="newPage">Le nouveau document à charger.</param>
    /// <param name="addHistory">True si la page d'origine devrait être ajoutée à l'historique de navigation, false sinon.</param>
    private void ChangeNavigationDocument(UIDocument newPage, bool addHistory)
    {
        HideQuickNavigation();
        newPage.gameObject.SetActive(true);
        VisualElement newRoot = newPage.rootVisualElement;

        newRoot.Q<Button>("BackButton").clicked += GoBack;
        newRoot.Q<Button>("HomeButton").clicked += GoToHome;
        newRoot.Q<Button>("QuickMenuButton").clicked += ShowQuickNavigation;

        if (addHistory)
            documentHistory.Add(currentDocument);

        if (currentDocument != newPage)
            currentDocument.gameObject.SetActive(false);
        currentDocument = newPage;

        switch (documentsReferences.IndexOf(newPage))
        {
            case 0:
                newRoot.Q<Button>("ShopButton").clicked += GoToShop;
                newRoot.Q<Button>("ARTestButton").clicked += GoToARTest;
                break;
            case 1:
                List<Furniture> allFurnitures = APIController.GetAllFurnitures().Current;   // À remplacer
                ScrollView scrollView = newRoot.Q<ScrollView>("FurnitureSelection");

                foreach (Furniture furniture in allFurnitures)
                {
                    Button furnitureDisplay = furnitureNavigationTemplate.CloneTree().Q<Button>(null, new string[] { "FurnitureDisplay", "navigation-button" });

                    furnitureDisplay.Q<VisualElement>(className: "FurniturePreview").style.backgroundImage = new StyleBackground(furniture.preview);
                    furnitureDisplay.Q<Label>(className: "FurnitureName").text = furniture.name;
                    furnitureDisplay.Q<Label>(className: "FurnitureDescription").text = furniture.description;
                    furnitureDisplay.Q<Label>(className: "FurniturePrice").text = furniture.price.ToString() + " €";

                    //furnitureDisplay.clicked += () => GoToFurnitureDetails(furniture);

                    scrollView.contentViewport.Add(furnitureDisplay);
                }
                break;
            case 2:
                newRoot.Q<VisualElement>("TitleContainer").Q<Label>().text = displayedFurniture.name;
                newRoot.Q<VisualElement>("Preview").style.backgroundImage = new StyleBackground(displayedFurniture.preview);
                newRoot.Q<Label>("Dimensions").text = "Dimensions : " + displayedFurniture.width + "m x " + displayedFurniture.length + "m x " + displayedFurniture.height + "m";
                newRoot.Q<Label>("Description").text = displayedFurniture.description;
                newRoot.Q<Label>("PriceTag").text = "Prix : " + displayedFurniture.price + " €";

                newRoot.Q<Button>("AddToCart").clicked += AddToCart;
                newRoot.Q<Button>("FurnitureTest").clicked += GoToARTest;
                break;
            case 3:
                DrawCartScrollView();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Cette méthode charge la page d'accueil de l'application, et ajoute la page d'origine dans l'historique de navigation.
    /// </summary>
    private void GoToHome()
    {
        if (currentDocument != documentsReferences[0])
            ChangeNavigationDocument(documentsReferences[0], true);
    }

    /// <summary>
    /// Cette méthode charge la page de magasin, et ajoute la page d'origine dans l'historique de navigation.
    /// </summary>
    private void GoToShop()
    {
        ChangeNavigationDocument(documentsReferences[1], true);
    }

    /// <summary>
    /// Cette méthode charge le mode test Réalité Augmentée, et cache la fenêtre de navigation courante.
    /// </summary>
    private void GoToARTest()
    {
        ApplicationManager.EnableARTestScene();
        currentDocument.gameObject.SetActive(false);
        documentsReferences[4].gameObject.SetActive(false);
        navigationCamera.gameObject.SetActive(false);
    }

    /// <summary>
    /// Cette méthode charge la page du mobilier sélectionné, et ajoute la page d'origine dans l'historique de navigation.
    /// </summary>
    private void GoToFurnitureDetails(FurnitureSO furniture)
    {
        displayedFurniture = furniture;
        ChangeNavigationDocument(documentsReferences[2], true);
    }

    /// <summary>
    /// Cette méthode réactive la page de navigation courante, et la caméra, typiquement en revenant du mode de test Réalité Augmentée.
    /// </summary>
    public static void ReenableNavigation()
    {
        _this.ChangeNavigationDocument(currentDocument, false);
        _this.navigationCamera.gameObject.SetActive(true);
    }

    /// <summary>
    /// Cette méthode ajoute le mobilier affiché dans le panier de l'utilisateur, puis affiche le panier de l'utilisateur, tout en ajoutant la page d'origine dans l'historique de navigation.
    /// </summary>
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
        ChangeNavigationDocument(documentsReferences[3], true);
    }

    /// <summary>
    /// Cette méthode charge la page du panier de l'utilisateur, et ajoute la page d'origine dans l'historique de navigation.
    /// </summary>
    private void GoToCart()
    {
        ChangeNavigationDocument(documentsReferences[3], true);
    }

    /// <summary>
    /// Cette méthode incrémente le nombre de mobiliers dans le panier de l'utilisateur, et recharge l'affichage du panier.
    /// </summary>
    /// <param name="furniture">Le mobilier à incrémenter dans le panier.</param>
    private void IncrementFurnitureCart(FurnitureSO furniture)
    {
        userCart[furniture]++;
        DrawCartScrollView();
    }

    /// <summary>
    /// Cette méthode décrémente le nombre de mobiliers dans le panier de l'utilisateur, et recharge l'affichage du panier. S'il n'y a qu'un seul exemplaire du meuble dans le panier, rien n'est fait.
    /// </summary>
    /// <param name="furniture">Le mobilier à décrémenter dans le panier.</param>
    private void DecrementFurnitureCart(FurnitureSO furniture)
    {
        if (userCart[furniture] > 1)
        {
            userCart[furniture]--;
            DrawCartScrollView();
        }
    }

    /// <summary>
    /// Cette méthode retire le mobilier du panier de l'utilisateur, et recharge l'affichage du panier.
    /// </summary>
    /// <param name="furniture">Le mobilier à incrémenter dans le panier.</param>
    private void RemoveFurnitureCart(FurnitureSO furniture)
    {
        userCart.Remove(furniture);
        DrawCartScrollView();
    }

    /// <summary>
    /// Cette méthode charge l'affichage du panier de l'utilisateur. Pour chaque meuble ajouté, il affiche l'image de prévisualisation, le nom, le nombre ajouté, et le prix individuel.
    /// Une fois cela fait, il additionne les valeurs cumulées de prix, et l'affiche en bas de l'écran.
    /// </summary>
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
            cartFurniture.Q<Label>(className: "Price").text = item.Key.price + " €";

            totalPrice += item.Key.price * item.Value;

            cartFurniture.Q<Button>(className: "ReduceQuantity").clicked += () => DecrementFurnitureCart(item.Key);
            cartFurniture.Q<Button>(className: "AugmentQuantity").clicked += () => IncrementFurnitureCart(item.Key);
            cartFurniture.Q<Button>(className: "Remove").clicked += () => RemoveFurnitureCart(item.Key);

            cartDisplay.Add(cartFurniture);
        }

        currentDocument.rootVisualElement.Q<Label>("TotalPrice").text = "Total : " + totalPrice + " €";
    }

    /// <summary>
    /// Cette méthode affiche la page de navigation rapide.
    /// </summary>
    private void ShowQuickNavigation()
    {
        documentsReferences[4].gameObject.SetActive(true);

        VisualElement menuRoot = documentsReferences[4].rootVisualElement;

        menuRoot.Q<Button>("InvisibleGoBackButton").clicked += HideQuickNavigation;
        menuRoot.Q<Button>("HomeButton").clicked += GoToHome;
        menuRoot.Q<Button>("ARButton").clicked += GoToARTest;
        menuRoot.Q<Button>("ShopButton").clicked += GoToShop;
        menuRoot.Q<Button>("CartButton").clicked += GoToCart;
    }

    /// <summary>
    /// Cette méthode cache la page de navigation rapide.
    /// </summary>
    private void HideQuickNavigation()
    {
        documentsReferences[4].gameObject.SetActive(false);
    }

    /// <summary>
    /// Cette méthode permet de charger la page de panier, tout en ajoutant les meubles placés en Réalité Augmenté dans le panier.
    /// </summary>
    /// <param name="placedFurnitures">La liste des meubles placés en Réalité Augmentée, et à ajouter dans le panier.</param>
    public static void FromARToCartWithItems(List<FurnitureSO> placedFurnitures)
    {
        ReenableNavigation();

        foreach (FurnitureSO furniture in placedFurnitures)
        {
            if (_this.userCart.ContainsKey(furniture))
                _this.userCart[furniture]++;
            else
                _this.userCart.Add(furniture, 1);
        }

        _this.GoToCart();
    }
}
