using UnityEngine;

public class WebViewExample : MonoBehaviour
{
    private WebViewQuad webViewQuad;

    void Start()
    {
        webViewQuad = GetComponent<WebViewQuad>();
        webViewQuad.LoadUrl("https://www.google.com");
    }
}
