using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationUIController : MonoBehaviour
{
    [SerializeField]
    UIDocument homeDocument;

    UIDocument currentDocument;

    // Start is called before the first frame update
    void Start()
    {
        currentDocument = homeDocument;
    }

    private void GoBack()
    {
        UIDocument previousDoc = currentDocument;
        GameObject.FindObjectOfType<UIDocument>().gameObject.SetActive(false);
        previousDoc.gameObject.SetActive(true);
    }

    private void OnNavigationDocumentChange(UIDocument newPage)
    {
        newPage.rootVisualElement.Q<Button>("BackButton").clicked += GoBack;

        newPage.gameObject.SetActive(true);
        currentDocument.gameObject.SetActive(false);
        currentDocument = newPage;
    }
}
