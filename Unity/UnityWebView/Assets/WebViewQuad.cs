using System.Collections.Generic;
using System;
using UnityEngine;

public class WebViewQuad : MonoBehaviour
{
    private AndroidJavaObject webViewPlugin;
    private Texture2D webViewTexture;
    private Queue<System.Action> executeOnMainThread = new Queue<System.Action>();

    void Start()
    {
        try
        {
            webViewPlugin = new AndroidJavaObject("com.morphyn.unity.WebViewPlugin", 1024, 768);
            webViewTexture = new Texture2D(1024, 768, TextureFormat.ARGB32, false);
            var renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.Log("Failed to get Renderer");
            }
            else
            {
                renderer.material.mainTexture = webViewTexture;
                Debug.Log("Texture applied to Renderer");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    
}

    void Update()
    {
        //Process the main thread queue
        while (executeOnMainThread.Count > 0)
        {
            executeOnMainThread.Dequeue().Invoke();
        }

        // Check if the WebView is ready to display
        bool isWebViewReady = webViewPlugin.Call<bool>("isWebViewReady");
        if (isWebViewReady)
        {
            UpdateWebViewTexture();
        }
    }

    private void UpdateWebViewTexture()
    {
        // get the updated WebView bitmap
        AndroidJavaObject webViewBitmap = webViewPlugin.Call<AndroidJavaObject>("getWebViewBitmap");

        if (webViewBitmap != null)
        {
            // convert the Android Bitmap to a Unity Texture2D
            sbyte[] bitmapData = webViewPlugin.Call<sbyte[]>("compressToJpeg", 100);
            byte[] bitmapDataByte = Array.ConvertAll(bitmapData, b => unchecked((byte)b));

            webViewTexture.LoadImage(bitmapDataByte);
        }
        else
        {
            Debug.Log("Cant find webViewBitmap");
        }
    }

    public void LoadUrl(string url)
    {
        // Dispatch to the main thread
        executeOnMainThread.Enqueue(() => {
            webViewPlugin.Call("loadUrl", url);
        });
    }
}
