using System;
using System.Collections.Generic;
using UnityEngine;


namespace MorphynWebView
{
    public class WorldWebView : MonoBehaviour
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
          
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
               
                if (hit.collider.gameObject == gameObject)
                {
                    // Transform the hit point from world space to local space
                    Vector3 localHitPoint = transform.InverseTransformPoint(hit.point);

                    // Transform the local coordinates to the range [0, 1]
                    float u = localHitPoint.x / transform.localScale.x + 0.5f;
                    float v = 1f - (localHitPoint.y / transform.localScale.y + 0.5f); // Invert the y-axis

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

}
