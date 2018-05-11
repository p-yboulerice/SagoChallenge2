package com.sagosago.sagobiz;

import android.app.Activity;

public class ParentsWebView extends WebView {
	protected static final String DIALOG_FRAGMENT_TAG = "ParentsWebViewDialogFragment";
	
	public ParentsWebView(Activity activity) {
		super(activity);
		this.dialogFragmentTag = DIALOG_FRAGMENT_TAG;
	}
}