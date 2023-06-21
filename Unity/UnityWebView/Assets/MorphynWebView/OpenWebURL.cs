using MorphynWebView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebURL : MonoBehaviour
{
    CanvasWebView canvasWebView;
    WorldWebView worldWebView;

    public string url;
    // Start is called before the first frame update
    void Start()
    {
        canvasWebView = FindObjectOfType<CanvasWebView>();
        worldWebView = FindObjectOfType<WorldWebView>();
        if (canvasWebView != null)
        {
            canvasWebView.LoadUrl(url);
        }
        else
        {
            if (worldWebView != null)
            {
                worldWebView.LoadUrl(url);
            }
          
        }
    }

}
