package com.morphyn.unity;

import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.os.Handler;
import android.os.Looper;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;

public class WebViewPlugin {
    private WebView webView;
    private Bitmap webViewBitmap;
    private Handler mainThreadHandler;
    private boolean isWebViewReady;

    public WebViewPlugin(final int width, final int height) {
        mainThreadHandler = new Handler(Looper.getMainLooper());

        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                webView = new WebView(UnityPlayer.currentActivity);
                webViewBitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);

                webView.setWebViewClient(new WebViewClient() {
                    @Override
                    public void onPageFinished(WebView view, String url) {
                        Canvas canvas = new Canvas(webViewBitmap);
                        view.draw(canvas);
                        isWebViewReady = true;
                    }
                });
            }
        });
    }

    public void loadUrl(final String url) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                webView.loadUrl(url);
            }
        });
    }

    public Bitmap getWebViewBitmap() {
        return webViewBitmap;
    }

    public byte[] compressToJpeg(int quality) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        webViewBitmap.compress(Bitmap.CompressFormat.JPEG, quality, stream);
        return stream.toByteArray();
    }
    public boolean isWebViewReady() {
        return isWebViewReady;
    }
}
