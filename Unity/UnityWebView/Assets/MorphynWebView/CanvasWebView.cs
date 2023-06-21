using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace MorphynWebView
{
    public class CanvasWebView: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private AndroidJavaObject webViewPlugin;
        private Texture2D webViewTexture;
        private Queue<System.Action> executeOnMainThread = new Queue<System.Action>();

        private RawImage webViewImage;

        void Start()
        {
            webViewImage = GetComponent<RawImage>();

            if (webViewImage == null )
            {
                webViewImage = this.AddComponent<RawImage>();
            }

            try
            {
                webViewPlugin = new AndroidJavaObject("com.morphyn.unity.WebViewPlugin", Screen.width, Screen.height);
                webViewTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
                webViewImage.texture = webViewTexture;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void Update()
        {
            while (executeOnMainThread.Count > 0)
            {
                executeOnMainThread.Dequeue().Invoke();
            }

            AndroidJavaObject webViewBitmap = webViewPlugin.Call<AndroidJavaObject>("getWebViewBitmap");

            if (webViewBitmap != null)
            {
                byte[] bitmapData = webViewPlugin.Call<byte[]>("compressToJpeg", webViewBitmap, 100);
                webViewTexture.LoadImage(bitmapData);
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                Vector2 normalizedPoint = new Vector2((localPoint.x - rectTransform.rect.x) / rectTransform.rect.width, 1 - ((localPoint.y - rectTransform.rect.y) / rectTransform.rect.height));
                webViewPlugin.Call("dragStart", normalizedPoint.x, normalizedPoint.y);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                Vector2 normalizedPoint = new Vector2((localPoint.x - rectTransform.rect.x) / rectTransform.rect.width, 1 - ((localPoint.y - rectTransform.rect.y) / rectTransform.rect.height));
                webViewPlugin.Call("dragEnd", normalizedPoint.x, normalizedPoint.y);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                Vector2 normalizedPoint = new Vector2((localPoint.x - rectTransform.rect.x) / rectTransform.rect.width, 1 - ((localPoint.y - rectTransform.rect.y) / rectTransform.rect.height));
                webViewPlugin.Call("dragTo", normalizedPoint.x, normalizedPoint.y);
            }
        }


        public void LoadUrl(string url)
        {
            executeOnMainThread.Enqueue(() => {
                webViewPlugin.Call("loadUrl", url);
            });
        }
    }
}