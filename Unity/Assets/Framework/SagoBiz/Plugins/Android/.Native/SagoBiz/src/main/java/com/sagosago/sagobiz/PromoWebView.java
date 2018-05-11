package com.sagosago.sagobiz;

import android.app.Activity;

public class PromoWebView extends WebView {
	protected static final String DIALOG_FRAGMENT_TAG = "PromoWebViewDialogFragment";
	
	public PromoWebView(Activity activity) {
		super(activity);
		this.dialogFragmentTag = DIALOG_FRAGMENT_TAG;
	}

	public static void initializeWebView(final Activity activity) {
		SagoBizDebug.log(DIALOG_FRAGMENT_TAG, "early initilization of WebView");

		activity.runOnUiThread(new Runnable() {
			
			@Override
			public void run() {
				if (WebViewDialogFragment.getWebView() == null) {
					WebViewDialogFragment.createWebViewInstance(activity);
				}
			}
		});
		
	}
	
	protected WebViewJavascriptInterface getWebViewJavascriptInterface() {
		return new PromoWebViewJavascriptInterface(PromoWebView.this);
	}

	private class PromoWebViewJavascriptInterface extends WebViewJavascriptInterface {
		public PromoWebViewJavascriptInterface(WebView webView) {
			super(webView);
		}
	}
}