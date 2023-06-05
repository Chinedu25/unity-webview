package com.morphyn.unity;

import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.os.Handler;
import android.os.Looper;
import android.view.View;
import android.util.Log;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;

public class WebViewPlugin {
    private WebView webView;
    private Bitmap webViewBitmap;
    private Handler mainThreadHandler;
    private boolean isWebViewReady;

    private  static  final String  LOGTAG = "MorphynWebView";

    public WebViewPlugin(final int width, final int height) {

        Log.i(LOGTAG, "Started plugin WebView");
        mainThreadHandler = new Handler(Looper.getMainLooper());


        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                webView = new WebView(UnityPlayer.currentActivity);
                webView.measure(View.MeasureSpec.makeMeasureSpec(width, View.MeasureSpec.EXACTLY), View.MeasureSpec.makeMeasureSpec(height, View.MeasureSpec.EXACTLY));
                webView.layout(0, 0, webView.getMeasuredWidth(), webView.getMeasuredHeight());
                webViewBitmap = Bitmap.createBitmap(webView.getMeasuredWidth(), webView.getMeasuredHeight(), Bitmap.Config.ARGB_8888);

                webView.setWebViewClient(new WebViewClient() {
                    @Override
                    public void onPageFinished(WebView view, String url) {
                        isWebViewReady = true;
                        Canvas canvas = new Canvas(webViewBitmap);
                        view.draw(canvas);
                        Log.i(LOGTAG, "Page load finished");
                    }
                });
            }
        });
    }

    public void loadUrl(final String url) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                Log.i(LOGTAG, "loading " + url);
                webView.loadUrl(url);
            }
        });
    }

    public Bitmap getWebViewBitmap() {
        return webViewBitmap;
    }

    public byte[] compressToJpeg(int quality) {
        Log.i(LOGTAG, "getting jpeg");
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        webViewBitmap.compress(Bitmap.CompressFormat.JPEG, quality, stream);
        return stream.toByteArray();
    }

    public boolean isWebViewReady() {
        return isWebViewReady;
    }
}
