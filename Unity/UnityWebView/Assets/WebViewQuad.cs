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
            webViewPlugin = new AndroidJavaObject("com.morphyn.unity.WebViewPlugin", Screen.width, Screen.height);
            webViewTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            GetComponent<Renderer>().material.mainTexture = webViewTexture;
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

        HandleWebViewTouch();

        AndroidJavaObject webViewBitmap = webViewPlugin.Call<AndroidJavaObject>("getWebViewBitmap");

        if (webViewBitmap != null)
        {
            byte[] bitmapData = webViewPlugin.Call<byte[]>("compressToJpeg", webViewBitmap, 100);
            webViewTexture.LoadImage(bitmapData);
        }
    }

    private void HandleWebViewTouch()
    {
        // Create a ray from the mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit was the quad with the WebView
            if (hit.collider.gameObject == gameObject)
            {
                // Transform the hit point from world space to local space
                Vector3 localHitPoint = transform.InverseTransformPoint(hit.point);

                // Transform the local coordinates to the range [0, 1]
                float u = localHitPoint.x / transform.localScale.x + 0.5f;
                float v = 1f - (localHitPoint.y / transform.localScale.y + 0.5f); // Invert the y-axis

                // Pass the normalized coordinates to the Android plugin
                //if (Input.GetMouseButton(0))
                //{
                //    webViewPlugin.Call("clickAt", u, v);
                //}

                if (Input.GetMouseButtonDown(0))
                {
                    webViewPlugin.Call("dragStart", u, v);
                }
                else if (Input.GetMouseButton(0))
                {
                    webViewPlugin.Call("dragTo", u, v);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    webViewPlugin.Call("dragEnd", u, v);
                }

            }
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
