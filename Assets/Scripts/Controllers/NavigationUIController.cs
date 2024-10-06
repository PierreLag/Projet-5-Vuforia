using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Threading.Tasks;
using System.Linq;

public class NavigationUIController : MonoBehaviour
{
    [SerializeField]
    List<UIDocument> documentsReferences;
    [SerializeField]
    VisualTreeAsset furnitureNavigationTemplate;
    [SerializeField]
    VisualTreeAsset cartFurnitureDisplayTemplate;
    [SerializeField]
    VisualTreeAsset adminFurnitureDisplayTemplate;
    [SerializeField]
    Camera navigationCamera;

    static NavigationUIController _this;
    static UIDocument currentDocument;

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
        documentHistory = new List<UIDocument>();
        userCart = new Dictionary<FurnitureSO, int>();

        VisualElement docRoot = currentDocument.rootVisualElement;
        docRoot.Q<Button>("BackButton").clicked += GoBack;
        docRoot.Q<Button>("QuickMenuButton").clicked += ShowQuickNavigation;
        docRoot.Q<Button>("ShopButton").clicked += GoToShop;
        docRoot.Q<Button>("ARTestButton").clicked += GoToARTest;

        if (ApplicationManager.GetUser() == null)
        {
            docRoot.Q<Button>("NoAccount").clicked += GoToLogin;
        }

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
    private async void ChangeNavigationDocument(UIDocument newPage, bool addHistory)
    {
        HideQuickNavigation();
        newPage.gameObject.SetActive(true);
        VisualElement newRoot = newPage.rootVisualElement;

        newRoot.Q<Button>("BackButton").clicked += GoBack;
        newRoot.Q<Button>("HomeButton").clicked += GoToHome;
        newRoot.Q<Button>("QuickMenuButton").clicked += ShowQuickNavigation;
        
        if (ApplicationManager.GetUser() == null)
        {
            newRoot.Q<Button>("NoAccount").clicked += GoToLogin;
        }
        else
        {
            Debug.Log(ApplicationManager.GetUser());
            newRoot.Q<Button>("NoAccount").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            newRoot.Q<Button>("Pfp").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            newRoot.Q<Label>("Username").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            newRoot.Q<Label>("Username").text = ApplicationManager.GetUser().GetUsername();
        }

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
                await ApplicationManager.UpdateAllFurnitures();
                CatalogueSO allFurnitures = ApplicationManager.GetFurnitureList();
                ScrollView scrollView = newRoot.Q<ScrollView>("FurnitureSelection");

                foreach (FurnitureSO furniture in allFurnitures.furnitures)
                {
                    Button furnitureDisplay = furnitureNavigationTemplate.CloneTree().Q<Button>(null, new string[] { "FurnitureDisplay", "navigation-button" });

                    furnitureDisplay.Q<VisualElement>(className: "FurniturePreview").style.backgroundImage = new StyleBackground(furniture.preview);
                    furnitureDisplay.Q<Label>(className: "FurnitureName").text = furniture.name;
                    furnitureDisplay.Q<Label>(className: "FurnitureDescription").text = furniture.description;
                    furnitureDisplay.Q<Label>(className: "FurniturePrice").text = furniture.price.ToString() + " €";

                    furnitureDisplay.clicked += () => GoToFurnitureDetails(furniture);

                    scrollView.contentViewport.Add(furnitureDisplay);
                }
                break;
            case 2:
                newRoot.Q<VisualElement>("TitleContainer").Q<Label>().text = displayedFurniture.name;
                newRoot.Q<VisualElement>("Preview").style.backgroundImage = new StyleBackground(displayedFurniture.preview);
                newRoot.Q<Label>("Dimensions").text = "Dimensions : " + displayedFurniture.width + "m x " + displayedFurniture.length + "m x " + displayedFurniture.height + "m";
                newRoot.Q<Label>("Category").text = "Catégorie : " + displayedFurniture.category;
                newRoot.Q<Label>("Description").text = displayedFurniture.description;
                newRoot.Q<Label>("PriceTag").text = "Prix : " + displayedFurniture.price + " €";

                newRoot.Q<Button>("AddToCart").clicked += AddToCart;
                newRoot.Q<Button>("FurnitureTest").clicked += GoToARTest;
                break;
            case 3:
                DrawCartScrollView();
                break;
            case 5:
                newRoot.Q<Button>("LoginButton").clicked += AttemptLogin;
                newRoot.Q<Button>("CreateAccount").clicked += GoToCreateAccount;
                break;
            case 6:
                newRoot.Q<Button>("CreateButton").clicked += AttemptCreate;
                newRoot.Q<Button>("Login").clicked += GoToLogin;
                break;
            case 7:
                await ApplicationManager.UpdateAllFurnitures();
                CatalogueSO allFurnituresAdmin = ApplicationManager.GetFurnitureList();
                ScrollView scrollViewAdmin = newRoot.Q<ScrollView>("FurnitureSelection");

                foreach (FurnitureSO furniture in allFurnituresAdmin.furnitures)
                {
                    VisualElement furnitureDisplay = adminFurnitureDisplayTemplate.CloneTree().Q<VisualElement>(null, new string[] { "FurnitureItem" });

                    furnitureDisplay.Q<VisualElement>(className: "FurniturePreview").style.backgroundImage = new StyleBackground(furniture.preview);
                    furnitureDisplay.Q<Label>(className: "FurnitureName").text = furniture.name;

                    FloatField priceField = furnitureDisplay.Q<FloatField>(className: "Price");
                    priceField.value = furniture.price;
                    priceField.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;

                    scrollViewAdmin.contentViewport.Add(furnitureDisplay);
                }

                newRoot.Q<Button>("SubmitUpdate").clicked += UpdatePrices;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Cette méthode charge la page d'accueil de l'application, et ajoute la page d'origine dans l'historique de navigation.
    /// </summary>
    public void GoToHome()
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

        User user = ApplicationManager.GetUser();
        if (user != null && user.GetRole() == UserRole.ADMIN)
        {
            Button adminButton = menuRoot.Q<Button>("AdminButton");
            adminButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            adminButton.clicked += GoToAdmin;
        }
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

    public static NavigationUIController GetThis()
    {
        return _this;
    }

    private void GoToLogin()
    {
        ChangeNavigationDocument(documentsReferences[5], true);
    }

    private void GoToCreateAccount()
    {
        ChangeNavigationDocument(documentsReferences[6], true);
    }

    private async void AttemptLogin()
    {
        VisualElement loginRoot = currentDocument.rootVisualElement;

        string username = loginRoot.Q<TextField>("UsernameField").text;
        string password = loginRoot.Q<TextField>("Password").text;

        Label errorLabel = loginRoot.Q<Label>("ErrorText");

        if (username == "" || password == "")
        {
            errorLabel.text = "Veuillez remplir tous les champs";
        }
        else
        {
            StartCoroutine(APIController.Login(username, password));

            int timesWaiting = 0;
            while (APIController.GetResponse() == null && timesWaiting <= 4)
            {
                await Task.Delay(100);
                timesWaiting++;
            }

            string response = (string)APIController.GetResponse();
            switch (response)
            {
                case "Success":
                    GoToHome();
                    break;
                case "Login mismatch":
                    errorLabel.text = "Identifiant ou mot de passe erroné";
                    break;
                case "Connection error":
                    errorLabel.text = "Erreur de connexion au serveur";
                    break;
            }

            APIController.ResetResponse();
        }
    }

    private async void AttemptCreate()
    {
        VisualElement createRoot = currentDocument.rootVisualElement;

        string username = createRoot.Q<TextField>("UsernameField").text;
        string password = createRoot.Q<TextField>("Password").text;
        string confirmPassword = createRoot.Q<TextField>("ConfirmPassword").text;

        Label errorLabel = createRoot.Q<Label>("ErrorText");

        if (username == "" || password == "")
        {
            errorLabel.text = "Veuillez remplir tous les champs";
        }
        else if (password != confirmPassword)
        {
            errorLabel.text = "Mot de passe n'est pas confirmé. Veuillez rentrer le même mot de passe";
        }
        else
        {
            StartCoroutine(APIController.CreateAccount(username, password));

            int timesWaiting = 0;
            while (APIController.GetResponse() == null && timesWaiting <= 4)
            {
                await Task.Delay(100);
                timesWaiting++;
            }

            string response = APIController.GetResponse().ToString();
            Debug.Log(response);
            switch (response)
            {
                case "Success":
                    GoToHome();
                    break;
                case "Already existing user":
                    errorLabel.text = "L'utilisateur existe déjà";
                    break;
                case "Connection error":
                    errorLabel.text = "Erreur de connexion au serveur";
                    break;
            }

            APIController.ResetResponse();
        }
    }

    private void GoToAdmin()
    {
        ChangeNavigationDocument(documentsReferences[7], true);
    }

    private async void UpdatePrices()
    {
        ScrollView furnitureDisplay = currentDocument.rootVisualElement.Q<ScrollView>();
        List<VisualElement> furnitureElements = furnitureDisplay.contentViewport.Children().ToList();

        Label messageLabel = currentDocument.rootVisualElement.Q<Label>("MessageDisplay");

        messageLabel.text = "";

        foreach (VisualElement furniture in furnitureElements)
        {
            if (furniture.ClassListContains("FurnitureItem"))
            {
                string name = furniture.Q<Label>(className: "FurnitureName").text;
                float price = furniture.Q<FloatField>().value;

                StartCoroutine(APIController.UpdateFurniturePrice(name, price));
            }
        }

        int timesWaiting = 0;
        object response = APIController.GetResponse();
        while (response == null || ((int)response < furnitureElements.Count-1 && timesWaiting < 10))
        {
            await Task.Delay(100);
            response = APIController.GetResponse();
            Debug.Log(response);
            timesWaiting++;
        }

        if (timesWaiting < 10)
        {
            messageLabel.text = "Prix mis à jour";
            messageLabel.style.color = Color.blue;
        }
        else
        {
            messageLabel.text = "Délai long. Possible erreur de connexion";
            messageLabel.style.color = Color.red;
        }

        APIController.ResetResponse();
    }
}
