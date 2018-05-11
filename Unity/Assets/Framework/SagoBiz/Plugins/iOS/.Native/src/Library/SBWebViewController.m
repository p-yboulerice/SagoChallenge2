//
//  ViewController.m
//  SagoPromoTest
//

#import "SBWebViewController.h"
#import "WebViewJavascriptBridge.h"
#import "SBDebug.h"

@interface SBWebViewController()
@property (readwrite) IBOutlet UIWebView* webView;
@property (readwrite) UIActivityIndicatorView* loadingIndicator;
@property WebViewJavascriptBridge* bridge;
@property (nonatomic) BOOL errorWhenLoading;
@property (nonatomic) BOOL shouldDismissWebViewWhenPaused;
@end


@implementation SBWebViewController

extern void UnitySendMessage(const char *, const char *, const char *);
extern void UnityPause( int pause );
extern void UnityWillResume();
extern void UnityWillPause();

static NSString* sagoBizWebClientPreferencesKey = @"sagoBizWebClientPreferences";

/*! Enumeration type for handling actions.
 */
typedef enum {
    /** Close Action: When the webview is being closed. */
    actionTypeClose,
    actionTypeEvent,
    actionTypeLink,
    actionTypeStore,
    actionTypeVideo,
    actionTypeOther,
    actionTypeInvalid
} actionType;

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    if((interfaceOrientation == UIInterfaceOrientationLandscapeLeft) ||
       (interfaceOrientation == UIInterfaceOrientationLandscapeRight))  {
        return YES;
    }
    return NO;
}

- (void)dealloc {
    [SBDebug log:@"SagoWebViewController is being deallocated"];
}

- (void)viewDidLoad {
    [super viewDidLoad];
    self.wantsFullScreenLayout = YES;
    CGRect mainWindowSize = [UIApplication sharedApplication].keyWindow.rootViewController.view.bounds;
    self.webView = [[UIWebView alloc] initWithFrame:mainWindowSize];
    self.webView.dataDetectorTypes = UIDataDetectorTypeNone;
    self.webView.scrollView.decelerationRate = UIScrollViewDecelerationRateNormal;
    self.webView.scrollView.bounces = NO;

//  Uncomment the following code to enable remote safari inspector.
//    if ([SBDebug isEnabled]) {
//        [NSClassFromString(@"WebView") performSelector:@selector(_enableRemoteInspector)];
//    }
    
    [self.view addSubview:self.webView];

    // Add loading indicator
    self.loadingIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
    self.loadingIndicator.color = [UIColor colorWithRed:(238.0/255) green:(77.0/255) blue:(133.0/255) alpha:1.0];
    self.loadingIndicator.frame = mainWindowSize;
    [self.view addSubview:self.loadingIndicator];
}

- (void)webViewDidFinishLoad:(UIWebView *)webView {
    [SBDebug log:@"Page loaded successfully"];
    [self removeLoadingIndicator];
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error {
    [SBDebug log:@"Could not load the web page"];
    [SBDebug log:[NSString stringWithFormat:@"Reason: %@", [error description]]];
    self.errorWhenLoading = YES;

    [self removeLoadingIndicator];
    
    if (error.code == NSURLErrorCancelled) {
        [self closeAction:nil];
    } else {
        UIAlertView* alert = [[UIAlertView alloc] initWithTitle:@"Oh no!"
                                                         message:[error localizedDescription]
                                                        delegate: self
                                               cancelButtonTitle: @"OK"
                                               otherButtonTitles: nil];
        [alert show];
    }
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)viewWillAppear:(BOOL)animated {
    [SBDebug log:@"Sending a message to Unity as SagoBiz.NativeWebViewWillAppear()"];
    UnitySendMessage("SagoBiz", "InvokeOnWebViewWillAppear", "");
    
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(applicationWillResignActive) name:UIApplicationWillResignActiveNotification object:nil];
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(applicationDidBecomeActive) name:UIApplicationDidBecomeActiveNotification object:nil];
    
    self.shouldDismissWebViewWhenPaused = YES;
    
    if ([SBDebug isEnabled]) {
        [WebViewJavascriptBridge enableLogging];
    }
    __weak SBWebViewController *safeSelf = self;

    if (!_bridge) {
        [SBDebug log:@"Registering the Javascript webview bridge"];
        
        _bridge = [WebViewJavascriptBridge bridgeForWebView:safeSelf.webView webViewDelegate:safeSelf handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"ObjC received message from JS: %@", data]];
            responseCallback(@"Response for message from ObjC");
        } resourceBundle:[NSBundle bundleWithPath:[NSBundle.mainBundle pathForResource:@"SagoBiz" ofType:@"bundle"]]];
        
        [_bridge registerHandler:@"loadUrl" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"loadUrl was called: %@", data]];
            responseCallback(@"Response from loadUrl");
            [safeSelf.webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:data]]];
        }];
        
        [_bridge registerHandler:@"loadUrlExternal" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"loadUrlExternal was called: %@", data]];
            responseCallback(@"Response from loadUrlExternal");
            [[UIApplication sharedApplication] openURL:[NSURL URLWithString: data]];
        }];
        
        [_bridge registerHandler:@"isNativeAvailable" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"isNativeAvailable native was called: %@", data]];
            responseCallback(@"true");
        }];
        
        [_bridge registerHandler:@"homeButtonIsAvailable" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"homeButtonIsAvailable native was called: %@", data]];
            responseCallback(@"true");
            safeSelf.shouldDismissWebViewWhenPaused = NO;
        }];
        
        [_bridge registerHandler:@"getPreferences" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"getPreferences was called: %@", data]];
            NSString* retrievedData = [safeSelf getPreferences:data];
            responseCallback(retrievedData);
        }];

        [_bridge registerHandler:@"overlayDidAppear" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"overlayDidAppear native was called: %@", data]
             ];
            responseCallback(@"true");
            [safeSelf enableScroll:NO];
        }];

        [_bridge registerHandler:@"overlayDidDisappear" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"overlayDidDisappear native was called: %@", data]];
            responseCallback(@"true");
            [safeSelf enableScroll:YES];
        }];
        
        [_bridge registerHandler:@"handleAction" handler:^(id data, WVJBResponseCallback responseCallback) {
            [SBDebug log:[NSString stringWithFormat:@"handleAction  was called: %@", data]];
            if ([data isKindOfClass:[NSDictionary class]]) {
                
                [SBDebug log:@"Received a dictionary"];
                switch ([safeSelf getActionType:data]) {
                    case actionTypeClose: {
                        responseCallback(@"Response from closeAction");
                        [safeSelf closeAction:data];
                        break;
                    }
                    case actionTypeLink: {
                        responseCallback(@"Response from linkAction");
                        [safeSelf urlExternalAction:data];
                        break;
                    }
                    case actionTypeEvent: {
                        responseCallback(@"Response from eventAction");
                        [safeSelf eventAction:data];
                        break;
                    }
                    case actionTypeStore: {
                        responseCallback(@"Response from storeAction");
                        [safeSelf openStoreAction:data];
                        break;
                    }
                    case actionTypeVideo: {
                        responseCallback(@"Response from videoAction");
                        [safeSelf videoAction:data];
                        break;
                    }
                    case actionTypeOther:
                    case actionTypeInvalid:
                    default: {
                        [SBDebug log:@"Invalid action"];
                        responseCallback(@"Invalid action");
                        break;
                    }
                }
                
            } else if ([data isKindOfClass:[NSArray class]]) {
                
                [SBDebug log:@"Received an array"];
                
            } else {
                [SBDebug log:@"Received string or invalid data"];
            }

            responseCallback(@"Callback from handleAction native!");
        }];
    } else {
        [SBDebug log:@"Javascript webview bridge is already registered. This message is harmless."];
    }
    
    [super viewWillAppear:NO];
}

- (void)viewDidAppear:(BOOL)animated {
    [SBDebug log:@"viewDidAppear"];
    [super viewDidAppear:NO];
}


- (void)applicationWillResignActive {
    if (self.shouldDismissWebViewWhenPaused) {
        [self closeAction:nil];
    }
}

- (void)applicationDidBecomeActive {
    [SBDebug log:@"Pausing Unity from WebView"];
    UnityPause(1);
}

- (void)viewWillDisappear:(BOOL)animated {
    
    
}

- (void)OpenURL:(NSString*)url {
    [self.webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:url]]];
    [self.loadingIndicator startAnimating];
}

- (void)OpenExternalUrl:(NSString*)url {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString: url]];
}

- (BOOL)KeyExistsInJSONObject:(id)jsonData :(NSString*)key {
    return [jsonData valueForKey:key] != nil;
}

/* Action methods */

- (actionType)getActionType:(id)jsonData {
    
    if ([jsonData valueForKey:@"action"] != nil) {
        
        NSString* actionType = [jsonData valueForKey:@"action"];
        
        if ([actionType isEqualToString:@"closeAction"]) {
            
            return actionTypeClose;
            
        } else if ([actionType isEqualToString:@"linkAction"]) {
            
            return actionTypeLink;
            
        } else if ([actionType isEqualToString:@"eventAction"]) {
            
            return actionTypeEvent;
            
        } else if ([actionType isEqualToString:@"storeAction"]) {
            
            return actionTypeStore;
            
        } else if ([actionType isEqualToString:@"videoAction"]) {
            
            return actionTypeVideo;
            
        } else {
            
            return actionTypeOther;
        }
        
    } else {
        return actionTypeInvalid;
    }
}

- (NSString *)serializeMessage:(id)message {
    return [[NSString alloc] initWithData:[NSJSONSerialization dataWithJSONObject:message options:0 error:nil] encoding:NSUTF8StringEncoding];
}

- (id)DeserializeMessageJSON:(NSString *)messageJSON {

    return [NSJSONSerialization JSONObjectWithData:[messageJSON dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:nil];
}



- (void)closeAction:(id)jsonData {
    
    [self eventAction:jsonData];
    
    [[NSNotificationCenter defaultCenter]removeObserver:self];
    
    if (!self.isModal) {
        
        _bridge = nil;

        [self.view removeFromSuperview];

        [self.webView stopLoading];
        [self.webView loadHTMLString:@"" baseURL:nil];
        self.webView.delegate = nil;
        [self.webView removeFromSuperview];
        self.webView = nil;
        
        [self removeFromParentViewController];
        
    } else {
        
        [self.webView removeFromSuperview];
        [self dismissViewControllerAnimated:YES completion:nil];
        
    }
    
    [SBDebug log:@"Sending a message to Unity as SagoBiz.NativeWebViewDidDisappear()"];

    if (self.errorWhenLoading) {
        UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "error");
    } else {
        UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "");
    }

    
    if (self.deallocateBlock) {
        [SBDebug log:@"Running deallocateBlock"];        
        
        self.deallocateBlock();
        self.deallocateBlock = nil;
    } else {
        [SBDebug log:@"Deallocate block was nil"];
    }
}

- (void)urlExternalAction:(id)jsonData {
    
    [self eventAction:jsonData];
    
    if ([jsonData valueForKey:@"web_url"] != nil) {
        [self OpenExternalUrl:[jsonData valueForKey:@"web_url"]];
    }
}

- (void)eventAction:(id)jsonData {
    
    if ([self KeyExistsInJSONObject:jsonData :@"analytics"]) {
        
        id analytics = [jsonData valueForKey:@"analytics"];
        if ([analytics isKindOfClass:[NSDictionary class]]) {
                        
            if ([self KeyExistsInJSONObject:analytics :@"event_name"]) {
                
                NSString* eventName = [analytics valueForKey:@"event_name"];
                [SBDebug log:[NSString stringWithFormat:@"Tracking %@...", eventName]];
                if ([self KeyExistsInJSONObject:analytics :@"event_properties"]) {
                    if ([[analytics valueForKey:@"event_properties"] isKindOfClass:[NSDictionary class]]) {
                        
                        NSDictionary* properties = [analytics valueForKey:@"event_properties"];
                        [SBMixpanel trackEvent:eventName withProperties:properties];
                    
                    } else {
                        [SBDebug log:@"Invalid JSON format for event properties."];
                    }
                } else {
                    
                    [SBMixpanel trackEvent:eventName];
                }
            }
        } else {
            [SBDebug log:[NSString stringWithFormat:@"The analytics data were received in form of %@", [analytics class]]];
        }
    }
 
    [self saveWebClientPreferences:jsonData];
    
}

- (void)videoAction:(id)jsonData {
    
    [self eventAction:jsonData];

}

- (void)openStoreAction:(id)jsonData {
    
    [self eventAction:jsonData];
    
    if ([jsonData valueForKey:@"store_url"] != nil) {
        [self OpenExternalUrl:[jsonData valueForKey:@"store_url"]];
    } else {
        [self urlExternalAction:jsonData];
    }
}

- (void)saveWebClientPreferences:(id)jsonData {
    [SBDebug log:@"Checking for web client preferences..."];
    if ([self KeyExistsInJSONObject:jsonData :@"save_to_preferences"]) {
        
        id webPreferences = [jsonData valueForKey:@"save_to_preferences"];
        if ([webPreferences isKindOfClass:[NSDictionary class]]) {
            
            [SBDebug log:@"The server preferences were received properly."];
            
            [SBDebug log:@"deserializing the nsuserdefault for web client preferences."];
            NSDictionary* currentPrefs = [self DeserializeMessageJSON:[self getPreferences:jsonData]];
            
            [SBDebug log:@"deserializing the newly recieved web client preferences."];
            NSDictionary* newPrefs = webPreferences;
            
            NSMutableDictionary* mergedPrefs = [NSMutableDictionary new];
            // Adding the current preferences
            [SBDebug log:@"Adding the current preferences to the merging dictionary"];
            if (currentPrefs != nil) {
                if ([currentPrefs count] != 0) {
                    [mergedPrefs addEntriesFromDictionary:currentPrefs];
                }
            }
            [SBDebug log:@"Adding the newly recieved preferences to the merging dictionary"];
            // Merging with the newly recieved preferences.
            if (newPrefs != nil) {
                if ([newPrefs count] != 0) {
                    [mergedPrefs addEntriesFromDictionary:newPrefs];
                }
            }
            
            // serialize, encode to base64 and save it to userdefaults
            [SBDebug log:@"Serializing the merged dictionary"];
            NSString* result= [self serializeMessage:[NSDictionary dictionaryWithDictionary:mergedPrefs]];
            [SBDebug log:[NSString stringWithFormat:@"The merged dictionary in json:\n%@", result]];
            
            NSData* encodedData = [result dataUsingEncoding:NSUTF8StringEncoding];
            
            
            NSString* encodedResult = [encodedData base64EncodedStringWithOptions:0];
            [SBDebug log:[NSString stringWithFormat:@"The merged dictionary encoded to base64:\n%@", encodedResult]];
            
            [SBDebug log:[NSString stringWithFormat:@"Saving the merged dictionary result to NSUserDefault with the key: %@", sagoBizWebClientPreferencesKey]];
            NSUserDefaults* userDefaults = [NSUserDefaults standardUserDefaults];
            [userDefaults setValue:encodedResult forKey:sagoBizWebClientPreferencesKey];
            
            [SBDebug log:@"Server preferences were successfully stored"];

        } else {
            [SBDebug log:@"Invalid JSON format for server preferences. Expected type: json dictionary"];
        }
    }
}

- (NSString*)getPreferences:(id)jsonData {
    
    NSUserDefaults* userDefaults = [NSUserDefaults standardUserDefaults];
    NSString* result = [userDefaults valueForKey:sagoBizWebClientPreferencesKey];
    if (result != nil) {
        
        if ([result isEqualToString:@""]) {
            return @"";
        }
        
        [SBDebug log:[NSString stringWithFormat:@"Base64: %@", result]];

        NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:result options:0];
        NSString *decodedString = [[NSString alloc] initWithData:decodedData encoding:NSUTF8StringEncoding];
        [SBDebug log:[NSString stringWithFormat:@"Base64: %@", result]];
        [SBDebug log:[NSString stringWithFormat:@"Decoded:\n%@", decodedString]];

        return decodedString;
    }
    
    return @"";
}

- (void)removeLoadingIndicator {
    [self.loadingIndicator stopAnimating];
    [self.loadingIndicator removeFromSuperview];
    self.loadingIndicator = nil;
}

- (void)enableScroll:(BOOL)enable {
    self.webView.scrollView.scrollEnabled = enable;
}

#pragma mark - UIAlertViewDelegate

- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex {
    [self closeAction:nil];
}


@end
