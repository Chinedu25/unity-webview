using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebViewQuad : MonoBehaviour
{
    private AndroidJavaObject webViewPlugin;
    private Texture2D webViewTexture;
    private Queue<System.Action> executeOnMainThread = new Queue<System.Action>();

    void Start()
    {
        webViewPlugin = new AndroidJavaObject("com.morphyn.unity.WebViewPlugin", 1024, 768);
        webViewTexture = new Texture2D(1024, 768, TextureFormat.ARGB32, false);
        GetComponent<Renderer>().material.mainTexture = webViewTexture;
    }

    void Update()
    {
        // Process the main thread queue
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

        // convert the Android Bitmap to a Unity Texture2D
        byte[] bitmapData = webViewBitmap.Call<byte[]>("compressToJpeg", 100);
        webViewTexture.LoadImage(bitmapData);
    }

    public void LoadUrl(string url)
    {
        // Dispatch to the main thread
        executeOnMainThread.Enqueue(() => {
            webViewPlugin.Call("loadUrl", url);
        });
    }
}
