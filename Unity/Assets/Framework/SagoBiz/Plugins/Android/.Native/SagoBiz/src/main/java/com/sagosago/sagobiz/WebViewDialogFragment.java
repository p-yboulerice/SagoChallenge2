package com.sagosago.sagobiz;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Dialog;
import android.app.DialogFragment;
import android.app.Fragment;
import android.content.DialogInterface;
import android.content.DialogInterface.OnKeyListener;
import android.content.DialogInterface.OnShowListener;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.os.Build;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebResourceError;
import android.webkit.WebResourceRequest;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;

public class WebViewDialogFragment extends DialogFragment {
	private static WebView webView;
	public static final String LOG_TAG = "WebViewDialogFragment";
	public static final String savedUrlKey = "savedUrl";
	private String url;
	public WebViewJavascriptInterface javaScriptInterface;
	public boolean webLoadError;
	public static boolean ShouldCloseWebViewOnPause;
	
	////////////////////
	// Internal classes
	public class ViewClient extends WebViewClient {
		//For API Level <23
		@SuppressWarnings("deprecation")
		public void onReceivedError (WebView view, int errorCode, String description, String failingUrl){
			if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) return;

			super.onReceivedError(view, errorCode, description, failingUrl);

			SagoBizDebug.logError(LOG_TAG, "Could not load web page resources.");
			SagoBizDebug.logError(LOG_TAG, "{ ErrorCode: " + errorCode + ", Description: " + description +", FailingUrl: " + failingUrl + " }");

			webLoadError = true;
		}

		//For API Level >=23
		public void onReceivedError(WebView view, WebResourceRequest request, WebResourceError error) {
			super.onReceivedError(view, request, error);

			if(Build.VERSION.SDK_INT < Build.VERSION_CODES.M) return;

			SagoBizDebug.logError(LOG_TAG, "Could not load web page resources.");
			SagoBizDebug.logError(LOG_TAG, "{ ErrorCode: " + error.getErrorCode() + ", Description: " + error.getDescription() + ", FailingUrl: " + request.getUrl() + " }");
			webLoadError = true;
		}

		public void onReceivedHttpError(WebView view, WebResourceRequest request, WebResourceResponse errorResponse){
			super.onReceivedHttpError(view, request, errorResponse);

			if(Build.VERSION.SDK_INT < Build.VERSION_CODES.M) return;

			SagoBizDebug.logError(LOG_TAG, "HTTP error during load.");
			SagoBizDebug.logError(LOG_TAG, "{ ErrorCode: " + errorResponse.getStatusCode() + ", Description: " + errorResponse.getReasonPhrase() +", FailingUrl: " + request.getUrl() + " }");
			webLoadError = true;
		}

		public void onPageStarted (WebView view, String url, Bitmap favicon) {
			super.onPageStarted(view, url, favicon);

			SagoBizDebug.log(LOG_TAG, "The page load was started.");
		}

		public void onPageFinished(WebView view, String url){
			super.onPageFinished(view, url);

			if (webLoadError == true) {
				webLoadError = false;
				SagoBizDebug.logError(LOG_TAG, "Could not load web page.");

				Fragment frag = getFragmentManager().findFragmentByTag(com.sagosago.sagobiz.WebView.dialogFragmentTag);
			    if (frag != null) {
			    	WebViewDialogFragment webViewFrag = (WebViewDialogFragment) frag;
			    	webViewFrag.dismiss();
			    	getFragmentManager().executePendingTransactions();
			    } else {
			    	SagoBizDebug.logError(LOG_TAG, "WebViewDialogFragment is null.");
			    }
		    	SagoBizDebug.log(LOG_TAG, "Sending a message to Unity as SagoBiz.NativeWebViewDidDisappear(error)");
		    	com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "error");

			} else {
				SagoBizDebug.log(LOG_TAG, "The page load was finished.");
			}
		}
	}

	////////////////////

	public static WebView getWebView(){
		return webView;
	}

	public static void createWebViewInstance(Activity activity){

		if(webView != null) {
			SagoBizDebug.logWarning(LOG_TAG, "webView is not null. createWebViewInstance should only be called once!");
		}

		webView = new WebView(activity);
		webView.clearCache(true);
		webView.setOverScrollMode(View.OVER_SCROLL_NEVER);

		WebSettings settings = webView.getSettings();
		settings.setJavaScriptEnabled(true);
		settings.setSupportZoom(false);
	}

	public WebViewDialogFragment() {
		SagoBizDebug.log(LOG_TAG, "WebViewDialogFragment was called.");
		if (url != null) {
			if (url.equals("")) {
				SagoBizDebug.log(LOG_TAG, "The url is empty.");
			}
		}
		
		if (this.javaScriptInterface == null) {
			this.javaScriptInterface = new WebViewJavascriptInterface();
		}
	}
	
	public void SaveUrlIntoSharedPrefs() {
        // Saving the url in case the OS decides to kill the process of this fragment.
        // Note that the fragment is active at this point therefore setArgument() cannot be called.
        if (this.url != null) {
        	Activity act = getActivity();
        	if (act != null)
        	{
		        SharedPreferences sharedPrefs = act.getSharedPreferences(act.getPackageName(), 0);
		        SharedPreferences.Editor editor = sharedPrefs.edit();
		        editor.putString(savedUrlKey, this.url);
		        editor.commit();
        	}
        }
	}
	
	@Override
    public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		if (url == null) {
			Bundle args = getArguments();
			String urlFromArguments = args.getString(savedUrlKey);
			if (urlFromArguments != null) {
				if(!urlFromArguments.isEmpty())
				{
					this.url = urlFromArguments;
				}
			} else if (savedInstanceState != null) {
				SagoBizDebug.log(LOG_TAG, "Retrieving url from savedInstanceState...");
				String savedUrl = savedInstanceState.getString(savedUrlKey, "");
				if (!savedUrl.isEmpty()) {
					this.url = savedUrl;
				}
			} else {
				SharedPreferences sharedPrefs = getActivity().getSharedPreferences(getActivity().getPackageName(), 0);
				String savedUrl = sharedPrefs.getString(savedUrlKey, "");
				if (!savedUrl.isEmpty()) {
					SagoBizDebug.log(LOG_TAG, "Retrieving url from sharedPreferences...");
					this.url = savedUrl;
					SharedPreferences.Editor editor = sharedPrefs.edit();
					editor.remove(savedUrlKey);
					editor.commit();
				}
			}
		} else {
			SaveUrlIntoSharedPrefs();
		}
		
		if (this.javaScriptInterface == null) {
			this.javaScriptInterface = new WebViewJavascriptInterface();
		}
		
		SagoBizDebug.log(LOG_TAG, "Setting fullscreen mode from sagobiz");
		setStyle(DialogFragment.STYLE_NORMAL, R.style.SagoBizWebViewFullScreenTheme);
		
	}
	
	public void setWebView(Activity activity) {
		SagoBizDebug.log(LOG_TAG, "Setting WebView");
		if (this.javaScriptInterface == null) {
			this.javaScriptInterface = new WebViewJavascriptInterface();
		}

		if(webView == null){
			createWebViewInstance(activity);
		}
		webView.setWebViewClient(new ViewClient());
		
		SagoBizDebug.log(LOG_TAG, "Enabling the WebView remote debugger");
		if (SagoBizDebug.isDebuggingEnabled() && Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
			webView.setWebContentsDebuggingEnabled(true);
		}
		
		SagoBizDebug.log(LOG_TAG, "Adding javascript interface");
		if (javaScriptInterface != null) {
			webView.addJavascriptInterface(javaScriptInterface, javaScriptInterface.getInterfaceName());
		} else {
			SagoBizDebug.logError(LOG_TAG, "javaScriptInterface is null");
		}
	}
	
	/**
     * Enables or disables immersive mode. Only applies to Android 4.4+ (android-19)
     * @param enabled
     */
    @TargetApi(Build.VERSION_CODES.KITKAT)
	private void setImmersiveMode(boolean enabled) {
		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT && enabled) {
			
			int flags = View.SYSTEM_UI_FLAG_LAYOUT_STABLE 
						| View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
			            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_FULLSCREEN
						| View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
		
			getDialog().getWindow().getDecorView().setSystemUiVisibility(flags);
		}
	}
    
	@Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		
		setWebView(getActivity());
		
		ShouldCloseWebViewOnPause = true;
		
		getDialog().setOnShowListener(new OnShowListener() {
			
			@Override
			public void onShow(DialogInterface dialog) {
				setImmersiveMode(true);
			}
		});
		
		SagoBizDebug.log(LOG_TAG, "Loading the url: " + url);
		webView.loadUrl(url);

        return webView;
	}
	
	@Override
	public void onSaveInstanceState(Bundle outState) {
		// the UI component values are saved here.
		super.onSaveInstanceState(outState); 
		// Saving the url in case the OS decides to kill the process of this fragment.
	    if (this.url != null) {
			outState.putString(savedUrlKey, this.url);
		}
	}
	
	@Override 
	public void onStart() {
        super.onStart();
        
        Dialog dialog = getDialog();
        if (dialog != null) {
        	int width = ViewGroup.LayoutParams.MATCH_PARENT;
        	int height = ViewGroup.LayoutParams.MATCH_PARENT;
        	dialog.getWindow().setLayout(width, height);
        }
    }
	
    @Override
    public void onPause() {
        super.onPause();
        // Saving the url in case the OS decides to kill the process of this fragment.
        // Note that the fragment is active at this point therefore setArgument() cannot be called.
        if (this.url != null) {
        	SaveUrlIntoSharedPrefs();
        }
        
        if (webView != null) {
        	webView.pauseTimers();
            webView.onPause();
            if (ShouldCloseWebViewOnPause) {
            	SagoBizDebug.logError(LOG_TAG, "Could not load home buttonn.");
				
				Fragment frag = getFragmentManager().findFragmentByTag(com.sagosago.sagobiz.WebView.dialogFragmentTag);
			    if (frag != null) {
			    	WebViewDialogFragment webViewFrag = (WebViewDialogFragment) frag;
			    	webViewFrag.dismiss();
			    } else {
			    	SagoBizDebug.logError(LOG_TAG, "WebViewDialogFragment is null.");
			    }
		    	SagoBizDebug.log(LOG_TAG, "Sending a message to Unity as SagoBiz.NativeWebViewDidDisappear(error)");
		    	com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "error");
            }
        }
    }	

    
    @Override
    public void onResume() {
        super.onResume();
        
        setImmersiveMode(true);
        
        getDialog().setOnKeyListener(new OnKeyListener() {
			
			@Override
			public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
				if (keyCode == android.view.KeyEvent.KEYCODE_BACK) {
					SagoBizDebug.log(LOG_TAG, "Sending a message to Unity as SagoBiz.NativeWebViewDidDisappear()");
			    	com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "");
        		}
        		return false;
			}
		});
		
        if (webView != null) {
        	webView.onResume();
        	webView.resumeTimers();
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (webView != null) {
			webView.removeAllViews();
        	webView.destroy();
        	webView = null;
        }
    }
    
//    @Override
//    public void onActivityResult(int requestCode, int resultCode, Intent data) {
//        if (webView != null) {
//
//        	webView.onActivityResult(requestCode, resultCode, data);
//        } else {
//        	SagoBizDebug.logError(LOG_TAG, "webView is null");
//        }
//    }
    
}