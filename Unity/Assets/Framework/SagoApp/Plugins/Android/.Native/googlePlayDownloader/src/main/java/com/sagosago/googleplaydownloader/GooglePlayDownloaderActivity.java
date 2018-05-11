package com.sagosago.googleplaydownloader;

import android.app.Activity;
import android.app.PendingIntent;
import android.os.Build;
import android.os.Build.VERSION;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.content.Intent;
import android.os.Messenger;
import android.content.pm.PackageManager.NameNotFoundException;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.TextView;
import android.provider.Settings;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import com.google.android.vending.expansion.downloader.*;
import com.unity3d.player.UnityPlayerActivity;
import com.sagosago.sagoapp.*;

public class GooglePlayDownloaderActivity extends Activity implements IDownloaderClient {

	private GooglePlayDownloaderProgressBar mPB;
	
	private TextView mStatusText;
         
    private int mState;
	
    private IDownloaderService mRemoteService;
	
    private IStub mDownloaderClientStub;
    
    private static final String LOG_TAG = "Downloader";
    
	private static final String EXP_PATH = "/Android/obb/";
    
	private static final String xwalkNativeLibrary = "libxwalkcore.so";
    
	private boolean runningUnity = false;
    
	private static boolean debugMode;
	
	public static final String DEBUG_PREFIX = "SAGOGooglePlayDownloader->";
	
	public static boolean isDebugMode() {
		return debugMode;
	}
	
	public static void log(String tag, String message) {
		if (debugMode) {
			Log.d(DEBUG_PREFIX + tag, message);
		}
	}

	public static void logError(String tag, String message, Exception ex) {
		if (debugMode) {
			Log.e(DEBUG_PREFIX + tag, message, ex);
		}
	}
	
    /*
     * (non-Javadoc)
     * @see android.app.Activity#onCreate(android.os.Bundle)
     * Check if the downloader should be bypassed. 
     * If true, it runs Unity.
     * If false, it retrieves the license public key and asks google play store if download is required and downloads if needed, otherwise it runs unity.
     * NOTE: Even if the files already exist on the end-user device, the app still communicates with the google play store on first run.
     * So Internet connectivity is needed on first check. 
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
    	
        super.onCreate(savedInstanceState);
		
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		
		try {			
			
			String useExpansionFileValue = getResources().getText(getResources().getIdentifier("google_play_use_expansion_file", "string", getPackageName())).toString().toLowerCase();
			boolean useExpansionFile = useExpansionFileValue.equals("true");
			
			String debugModeValue = getResources().getText(getResources().getIdentifier("google_play_debug_expansion_file", "string", getPackageName())).toString().toLowerCase();
			debugMode = debugModeValue.equalsIgnoreCase("true");
			
			GooglePlayDownloaderActivity.log(LOG_TAG, "Interpreted as " + useExpansionFile);
			
			if (!useExpansionFile || 
				(useExpansionFile && expansionFileExists())) {
					GooglePlayDownloaderActivity.log(LOG_TAG, "Skipping download");
					runUnityBootstrap();
					return;
			} else {
				GooglePlayDownloaderService.BASE64_PUBLIC_KEY = getResources().getText(getResources().getIdentifier("google_play_license_public_key", "string", getPackageName())).toString();
				
				// This launches the download activity pending intent.
				Intent intentToLaunchMainActivityFromNotification = new Intent(this, this.getClass());
				intentToLaunchMainActivityFromNotification.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
				intentToLaunchMainActivityFromNotification.setAction("android.intent.action.MAIN");
				intentToLaunchMainActivityFromNotification.addCategory("android.intent.category.LAUNCHER");
				
				// Build PendingIntent used to open this activity from Notification
				 PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intentToLaunchMainActivityFromNotification, PendingIntent.FLAG_UPDATE_CURRENT);
				
				// Request to start the download
				int startResult = DownloaderClientMarshaller.startDownloadServiceIfRequired(this, pendingIntent, GooglePlayDownloaderService.class);

				GooglePlayDownloaderActivity.log(LOG_TAG, "Requesting download from the service.");
				if (startResult != DownloaderClientMarshaller.NO_DOWNLOAD_REQUIRED) {
					GooglePlayDownloaderActivity.log(LOG_TAG, "Download is required.");
					// The DownloaderService has started downloading the files, show progress
					initializeDownloadUI();
					return;
				} else { 
					// otherwise, download not needed so we fall through to starting the movie
					GooglePlayDownloaderActivity.log(LOG_TAG, "Downloading the expansion file is not needed");
					runUnityBootstrap();
					return;
				}
			}

		} catch (Exception ex) {
			GooglePlayDownloaderActivity.logError(LOG_TAG, "Error during the download:", ex);
			finish();
		}
		
    }
    
    /*
     * Runs Unity activity
     */
    protected void runUnity() {
    	
    	if (!runningUnity) {
    		runningUnity = true;
	    	GooglePlayDownloaderActivity.log(LOG_TAG, "Running unity activity");
			Intent unityIntent = new Intent(this, UnityPlayerActivity.class );
			unityIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
			startActivity(unityIntent);
			GooglePlayDownloaderActivity.log(LOG_TAG, "Finishing downloader activity");
			finish();
    	}
    }
    
    protected void runUnityBootstrap() {
    	
    	if (!runningUnity) {
    		runningUnity = true;
	    	GooglePlayDownloaderActivity.log(LOG_TAG, "Running Bootstrap activity");
			Intent bootstrapIntent = new Intent(this, Bootstrap.class );
			startActivity(bootstrapIntent);
			GooglePlayDownloaderActivity.log(LOG_TAG, "Finishing downloader activity");
			finish();
    	}
    }
    
    /**
     * Connect the stub to our service on resume.
     */
    @Override
    protected void onResume() {
    	
        if (null != mDownloaderClientStub) {
            mDownloaderClientStub.connect(this);
        }
        
        super.onResume();
        
        if (Build.VERSION.SDK_INT >= 19) {
	        // Activating immersive mode 
			int flags = View.SYSTEM_UI_FLAG_LAYOUT_STABLE 
			            | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
			            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_FULLSCREEN
			            | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
			
			getWindow().getDecorView().setSystemUiVisibility(flags);
        }
    }
	
    /**
     * Updating flags to keep the downloader in immersive mode.
     */
    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
    	super.onWindowFocusChanged(hasFocus);
        
    	if (Build.VERSION.SDK_INT >= 19) {
	    	if (hasFocus) {
	        	getWindow().getDecorView().setSystemUiVisibility(
		            View.SYSTEM_UI_FLAG_LAYOUT_STABLE 
		            | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
		            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
		            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
		            | View.SYSTEM_UI_FLAG_FULLSCREEN
		            | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY);
	        }
    	}
    }
    
    /**
     * Disconnect the stub from our service on stop
     */
    @Override
    protected void onStop() {
    	
        if (null != mDownloaderClientStub) {
            mDownloaderClientStub.disconnect(this);
        }

        super.onStop();
        
    }

    /*
     * Initializes the downloader UI
     */
    private void initializeDownloadUI() {

    	mDownloaderClientStub = DownloaderClientMarshaller.CreateStub(this, GooglePlayDownloaderService.class);
        
        // Remove notification bar
        this.getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(Helpers.getLayoutResource(this, "main"));
		
       	mPB = (GooglePlayDownloaderProgressBar) findViewById(Helpers.getIdResource(this, "sagoProgressBar"));
       	mStatusText = (TextView) findViewById(Helpers.getIdResource(this, "downloadingTextView"));
		
    }
    
    /*
     * Writes the current state on the UI
     */
    private void setState(int newState) {

    	if (mState != newState) {
            mState = newState;
            mStatusText.setText(Helpers.getDownloaderStringResourceIDFromState(this, newState));
        }
        
    }
	
    /*
     * (non-Javadoc)
     * @see com.google.android.vending.expansion.downloader.IDownloaderClient#onServiceConnected(android.os.Messenger)
     */
	@Override
	public void onServiceConnected(Messenger m) {
		
        mRemoteService = DownloaderServiceMarshaller.CreateProxy(m);
        mRemoteService.onClientUpdated(mDownloaderClientStub.getMessenger());
        
	}
	
	/*
	 * (non-Javadoc)
	 * @see com.google.android.vending.expansion.downloader.IDownloaderClient#onDownloadStateChanged(int)
	 * Checks if the download is completed and runs Unity activity
	 */
	@Override
	public void onDownloadStateChanged(int newState) {
		
        setState(newState);

        if (newState == IDownloaderClient.STATE_COMPLETED) {
	        mPB.setProgress(100);
			runUnityBootstrap();
        }
        
	}
	
	/*
	 * (non-Javadoc)
	 * @see com.google.android.vending.expansion.downloader.IDownloaderClient#onDownloadProgress(com.google.android.vending.expansion.downloader.DownloadProgressInfo)
	 * Updates the progress on the UI
	 */
	@Override
	public void onDownloadProgress(DownloadProgressInfo progress) {
		
        progress.mOverallTotal = progress.mOverallTotal;
        float p = ((float)progress.mOverallProgress/(float)progress.mOverallTotal)*100.0f;
        mPB.setProgress((int) p);
        
	}
	
	
	protected boolean expansionFileExists() {
		 if( Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED) ) {
		        // Build the full path to the app's expansion files
		        File root = Environment.getExternalStorageDirectory();
		        File expPath = new File(root.toString() + EXP_PATH + getPackageName() );
				try {
					File expFile = new File( expPath, Helpers.getExpansionAPKFileName( getApplicationContext(), true, getPackageManager().getPackageInfo( getPackageName(), 0 ).versionCode ) );
					if( expFile != null ) {
						GooglePlayDownloaderActivity.log( LOG_TAG, "XAPK: Version: " + getPackageManager().getPackageInfo( getPackageName(), 0 ).versionCode + " - " + expFile.getAbsolutePath() );
			        	return expFile.exists();
					}
				} catch (NameNotFoundException e) {
					GooglePlayDownloaderActivity.logError(LOG_TAG, "NameNotFoundException", e);
				}
		 }
		 return false;
	}
	
	protected String getExpansionPath() {
		if( Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED) ) {
			File root = Environment.getExternalStorageDirectory();
	        String expPath = root.toString() + EXP_PATH + getPackageName();
	        return expPath;
		}
		return "";
	}
	
	protected File getExpansionFile() {
		if( Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED) ) {
	        // Build the full path to the app's expansion files
	        File root = Environment.getExternalStorageDirectory();
	        File expPath = new File(root.toString() + EXP_PATH + getPackageName() );
			try {
				File expFile = new File( expPath, Helpers.getExpansionAPKFileName( getApplicationContext(), true, getPackageManager().getPackageInfo( getPackageName(), 0 ).versionCode ) );
				if( expFile != null ) {
					GooglePlayDownloaderActivity.log(LOG_TAG, "Expansion file path is: " + expFile.getAbsolutePath());
		        	return expFile;
				}
				
			} catch (NameNotFoundException e) {
				GooglePlayDownloaderActivity.logError(LOG_TAG, "NameNotFoundException", e);
			}
		}
		return null;
	}
	
	protected boolean unpackZip(File zipFile, String sourceFileName, String targetPath, String targetFileName) {       
	     InputStream is;
	     ZipInputStream zis;
	     try {
	         String filename;
	         is = new FileInputStream(zipFile); // new FileInputStream(path + zipName);
	         zis = new ZipInputStream(new BufferedInputStream(is));          
	         ZipEntry ze;
	         byte[] buffer = new byte[1024];
	         int count;
	         
	         if (!targetPath.endsWith("/")) {
	        	 targetPath += "/";
	         }
	         
	         while ((ze = zis.getNextEntry()) != null) {
	             try {
		             filename = ze.getName();
		             GooglePlayDownloaderActivity.log(LOG_TAG, zipFile.getName() + ": " + filename);
		             if (filename.equals(sourceFileName) && !ze.isDirectory()) {
		            	 GooglePlayDownloaderActivity.log(LOG_TAG, sourceFileName + " was found!");
		            	 GooglePlayDownloaderActivity.log(LOG_TAG, "Copying to " + targetPath + targetFileName);
			             FileOutputStream fout = new FileOutputStream(targetPath + targetFileName);
			             
			             while ((count = zis.read(buffer)) != -1) {
			                 fout.write(buffer, 0, count);             
			             }
			             GooglePlayDownloaderActivity.log(LOG_TAG, sourceFileName + " was copied successfully.");
			             fout.close();
			             File extractedFile = new File(targetPath + targetFileName);
			             extractedFile.setExecutable(true);
			             extractedFile.setReadOnly();
			             zis.closeEntry();
			             break;
		             }
		             zis.closeEntry();
	             } catch (Exception ex) {
	            	 GooglePlayDownloaderActivity.logError(LOG_TAG, "Error while reading " + ze.getName() + "from the zip file: ", ex);
	             }
	         }

	         zis.close();
	         
	     } catch(IOException e) {
	    	 GooglePlayDownloaderActivity.logError(LOG_TAG, "IOException", e);
	         return false;
	     }

	    return true;
	}
}
