using UnityEngine;

public class WebViewExample : MonoBehaviour
{
    private WebViewQuad webViewQuad;

    void Start()
    {
        webViewQuad = GetComponent<WebViewQuad>();
        webViewQuad.LoadUrl("http://www.google.com");
    }
}
