using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigateWeb : MonoBehaviour
{
    [SerializeField]TMP_InputField urlInput;
   [SerializeField] Button navigateButton;

    private void Start()
    {
        navigateButton.onClick.AddListener(GoToPage);
    }

    private void GoToPage()
    {
        FindObjectOfType<WebViewQuad>().LoadUrl(urlInput.text);
    }
}
