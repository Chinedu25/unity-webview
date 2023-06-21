package com.morphyn.unity;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.graphics.ImageFormat;
import android.graphics.PixelFormat;
import android.graphics.Rect;
import android.hardware.display.DisplayManager;
import android.hardware.display.VirtualDisplay;
import android.media.Image;
import android.media.ImageReader;
import android.os.Handler;
import android.os.Looper;
import android.os.Bundle;
import android.util.Log;
import android.view.Display;
import android.view.MotionEvent;
import android.view.View;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.app.Presentation;
import android.view.WindowManager;
import com.unity3d.player.UnityPlayer;
import android.os.SystemClock;
import java.nio.ByteBuffer;
import java.io.ByteArrayOutputStream;

public class WebViewPlugin {
    private WebView webView;
    private Handler mainThreadHandler;
    private ImageReader imageReader;
    private VirtualDisplay virtualDisplay;
    private Presentation presentation;
    private static final String LOGTAG = "MorphynWebView";

    public WebViewPlugin(final int width, final int height) {
        Log.i(LOGTAG, "Started plugin WebView");
        mainThreadHandler = new Handler(Looper.getMainLooper());

        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                Context context = UnityPlayer.currentActivity;

                webView = new WebView(context);
                webView.setLayerType(View.LAYER_TYPE_HARDWARE, null);
                webView.getSettings().setJavaScriptEnabled(true);
                webView.setWebChromeClient(new WebChromeClient());

                webView.setWebViewClient(new WebViewClient() {
                    @Override
                    public void onPageFinished(WebView view, String url) {
                        Log.i(LOGTAG, "Page load finished");
                    }
                });

                imageReader = ImageReader.newInstance(width, height, PixelFormat.RGBA_8888, 2);

                DisplayManager displayManager = (DisplayManager) context.getSystemService(Context.DISPLAY_SERVICE);
                virtualDisplay = displayManager.createVirtualDisplay(
                        "WebViewCapture",
                        width,
                        height,
                        context.getResources().getDisplayMetrics().densityDpi,
                        imageReader.getSurface(),
                        0
                );

                presentation = new Presentation(context, virtualDisplay.getDisplay()) {
                    @Override
                    protected void onCreate(Bundle savedInstanceState) {
                        super.onCreate(savedInstanceState);

                        setContentView(webView);
                    }
                };

                presentation.show();
            }
        });
    }

    public Bitmap getWebViewBitmap() {
        Image image = imageReader.acquireLatestImage();
        if (image != null) {
            final Image.Plane[] planes = image.getPlanes();
            final ByteBuffer buffer = planes[0].getBuffer();
            int pixelStride = planes[0].getPixelStride();
            int rowStride = planes[0].getRowStride();
            int rowPadding = rowStride - pixelStride * image.getWidth();

            Bitmap bitmap = Bitmap.createBitmap(image.getWidth() + rowPadding / pixelStride, image.getHeight(), Bitmap.Config.ARGB_8888);
            bitmap.copyPixelsFromBuffer(buffer);

            image.close();
            return bitmap;
        }
        return null;
    }
    public void dragStart(final float u, final float v) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                int x = (int) (u * webView.getMeasuredWidth());
                int y = (int) (v * webView.getMeasuredHeight());
                long time = SystemClock.uptimeMillis();
                webView.dispatchTouchEvent(MotionEvent.obtain(time, time, MotionEvent.ACTION_DOWN, x, y, 0));
            }
        });
    }

    public void dragTo(final float u, final float v) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                int x = (int) (u * webView.getMeasuredWidth());
                int y = (int) (v * webView.getMeasuredHeight());
                long time = SystemClock.uptimeMillis();
                webView.dispatchTouchEvent(MotionEvent.obtain(time, time, MotionEvent.ACTION_MOVE, x, y, 0));
            }
        });
    }

    public void dragEnd(final float u, final float v) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                int x = (int) (u * webView.getMeasuredWidth());
                int y = (int) (v * webView.getMeasuredHeight());
                long time = SystemClock.uptimeMillis();
                webView.dispatchTouchEvent(MotionEvent.obtain(time, time, MotionEvent.ACTION_UP, x, y, 0));
            }
        });
    }

    public byte[] compressToJpeg(Bitmap bitmap, int quality) {
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        bitmap.compress(CompressFormat.JPEG, quality, byteArrayOutputStream);
        return byteArrayOutputStream.toByteArray();
    }

    public void clickAt(final float u, final float v) {
        mainThreadHandler.post(new Runnable() {
            @Override
            public void run() {
                // Convert the normalized coordinates to WebView pixel coordinates
                int x = (int) (u * webView.getMeasuredWidth());
                int y = (int) (v * webView.getMeasuredHeight());

                // Dispatch a synthetic mouse down and mouse up event
                long time = SystemClock.uptimeMillis();
                webView.dispatchTouchEvent(MotionEvent.obtain(time, time, MotionEvent.ACTION_DOWN, x, y, 0));
                webView.dispatchTouchEvent(MotionEvent.obtain(time, time, MotionEvent.ACTION_UP, x, y, 0));
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

    public void destroy() {
        presentation.dismiss();
        virtualDisplay.release();
        imageReader.close();
    }
}
