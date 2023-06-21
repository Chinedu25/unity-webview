using MorphynWebView;
using System.Collections;
using UnityEngine;

public class WebViewExample : MonoBehaviour
{
    private WebViewQuad webViewQuad;
    public CanvasWebView webView;

    void Start()
    {
        webViewQuad = GetComponent<WebViewQuad>();


        StartCoroutine(waitToOpen());

    }

    IEnumerator waitToOpen()
    {
        yield return new WaitForSeconds(1);
        //webViewQuad.LoadUrl("https://www.youtube.com/watch?v=5AKl_cEB26c");
        if (webView != null)
        {
            webView.LoadUrl("https://www.youtube.com/watch?v=5AKl_cEB26c");
        }
    }
}
