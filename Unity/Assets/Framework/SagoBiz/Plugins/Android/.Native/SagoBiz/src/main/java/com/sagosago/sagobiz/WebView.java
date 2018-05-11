package com.sagosago.sagobiz;

import android.app.Activity;
import android.app.FragmentManager;
import android.os.Bundle;

public abstract class WebView {
	protected static final String DIALOG_FRAGMENT_TAG = "WebViewDialogFragment";
	
	protected Activity activity;
	public static String dialogFragmentTag = DIALOG_FRAGMENT_TAG;	
	protected WebViewDialogFragment webViewDialogFragment;
		
	public WebView(Activity activity) {
		this.activity = activity;
	}
	
	protected WebViewJavascriptInterface getWebViewJavascriptInterface() {
		return new WebViewJavascriptInterface(WebView.this);
	}
	
	public void display(final String url) {
		SagoBizDebug.log("WebView", "WebView display");
		SagoBizDebug.log(DIALOG_FRAGMENT_TAG, "Sending a message to Unity as SagoBiz.InvokeOnWebViewWillAppear()");
		com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBiz", "InvokeOnWebViewWillAppear", "");
		activity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				webViewDialogFragment = new WebViewDialogFragment();
				
				if (webViewDialogFragment != null) {
					Bundle args = new Bundle();
					args.putString(WebViewDialogFragment.savedUrlKey, url);
					webViewDialogFragment.setArguments(args);
					webViewDialogFragment.javaScriptInterface = getWebViewJavascriptInterface();
					
					FragmentManager fm;
					fm = activity.getFragmentManager();
					SagoBizDebug.log("WebView", "Showing webview");
					webViewDialogFragment.show(fm, WebView.this.dialogFragmentTag);
				} else {
					SagoBizDebug.logError("WebView", "webViewDialogFragment is null");
				}
			}
		});
	}
		
	public void close() {
		SagoBizDebug.log("WebView", "WebView close");
		webViewDialogFragment.dismiss();
	}
}
