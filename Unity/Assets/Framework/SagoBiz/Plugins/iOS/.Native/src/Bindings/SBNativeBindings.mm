//
//  SBNativeBindings.m
//  SBNativeBindings
//
//  Created by Aram on 2015-06-11.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "SBNativeBindings.h"
#import <SagoBiz/SBWebViewController.h>
#import <SagoBiz/SBDeviceInfo.h>
#import <SagoBiz/SBMixpanel.h>
#import <SagoBiz/SBDebug.h>
#import <SagoBiz/SBAnalyticsAppExit.h>
#import <SagoBiz/SBAppleSearchAd.h>
#import <SagoBiz/SBAdjust.h>

@interface SBNativeBindings()
@property (nonatomic, retain) SBWebViewController *webViewController;
@property (nonatomic, retain) SBAnalyticsAppExit *analyticsAppExit;
@end


@implementation SBNativeBindings

extern void UnityPause( int pause );
extern void UnityWillResume();
extern void UnityWillPause();

static SBNativeBindings* delegateObject = nil;

+ (id)sharedInstance {
    static dispatch_once_t pred = 0;
    static id instance = nil;
    
    dispatch_once(&pred, ^{
        instance = [[self alloc] init];
    });
    
    return instance;
}

- (id)init {
    if (self = [super init]) {
        self.analyticsAppExit = [SBAnalyticsAppExit new];
    }

    return self;
}

- (void)openWeb:(NSString*)url :(BOOL)isModal {
    
    // Pausing Unity when opening the UIWebView.
    UnityPause(1);
    
    self.webViewController = [[SBWebViewController new] autorelease];
    CGRect mainWindowSize = [UIApplication sharedApplication].keyWindow.rootViewController.view.bounds;
    self.webViewController.view.frame = mainWindowSize;
    self.webViewController.isModal = isModal;
    
    // Deallocating the webviewcontroller after we're done with it. This code block is called from WebViewController.closeAction()
    __unsafe_unretained SBNativeBindings* weakSelf = self;
    self.webViewController.deallocateBlock = ^ {
        [SBDebug log:@"Deallocating webview controller from SBNativeBinder"];
        weakSelf.webViewController = nil;
        // Un-Pausing Unity after closing the UIWebView.
        UnityPause(0);
    };
    
    [self.webViewController OpenURL:url];
    
    [SBDebug log:[NSString stringWithFormat:@"Is modal? %@", isModal ? @"YES" : @"NO"]];
    
    if (!isModal){
        // Loading the webview as a child view controller.
        [[UIApplication sharedApplication].keyWindow.rootViewController addChildViewController:self.webViewController];
        
        [[UIApplication sharedApplication].keyWindow.rootViewController.view addSubview:self.webViewController.view];
        
        [self.webViewController didMoveToParentViewController:[UIApplication sharedApplication].keyWindow.rootViewController];
        
    } else {
        // Loading the webview as a modal view controller.
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:self.webViewController animated:NO completion:nil];
    }
}

+ (NSArray*)DeserializeMessageJSON:(NSString *)messageJSON {
    return [NSJSONSerialization JSONObjectWithData:[messageJSON dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:nil];
}

// Converts C style string to NSString
NSString* CreateNSString (const char* string) {
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}


char* MakeCharArray(NSString *string) {
    //char* test = [string UTF8String];
    
    return strdup([string UTF8String]);
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string) {
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}


extern "C" {
    
    void _EnableDebugging() {
        [SBDebug setEnabled:YES];
    }

    void _DisableDebugging() {
        [SBDebug setEnabled:NO];
    }
    
    bool _IsDebuggingEnabled() {
        BOOL debugMode = [SBDebug isEnabled];
        
        if (debugMode == YES) {
            return true;
        } else {
            return false;
        }
    }
    
    void _OpenExternalWeb(char* url) {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString: CreateNSString(url)]];
    }
    
    void _OpenWeb(char* url, bool isModal) {
        BOOL modal = isModal ? YES : NO;
        [[SBNativeBindings sharedInstance] openWeb: CreateNSString(url): modal];
    }
    
    char* _BundleId() {
        return MakeCharArray([SBDeviceInfo BundleId]);
    }
    
    char* _BundleVersion() {
        return MakeCharArray([SBDeviceInfo BundleVersion]);
    }

    char* _BundleVersionCode() {
        return MakeCharArray([SBDeviceInfo BundleVersionCode]);
    }
    
    char* _AppName() {
        return MakeCharArray([SBDeviceInfo AppName]);
    }
    
    char* _Language() {
        return MakeCharArray([SBDeviceInfo Language]);
    }
    
    char* _LanguageCode() {
        return MakeCharArray([SBDeviceInfo LanguageCode]);
    }
    
    char* _CountryCode() {
        return MakeCharArray([SBDeviceInfo CountryCode]);
    }
    
    char* _OperatingSystem() {
        return MakeCharArray([SBDeviceInfo OperatingSystem]);
    }
    
    char* _OperatingSystemVersion() {
        return MakeCharArray([SBDeviceInfo OperatingSystemVersion]);
    }
    
    BOOL _HasCamera() {
        return [SBDeviceInfo HasCamera];
    }
    
    char* _TimeZone() {
        return MakeCharArray([SBDeviceInfo TimeZone]);
    }
    
    char* _UrlScheme() {
        return MakeCharArray([SBDeviceInfo UrlScheme]);
    }
    
    char* _VendorId() {
        return MakeCharArray([SBDeviceInfo VendorId]);
    }
    
    char* _GetInstalledAppsWithUrlSchemes(char* urlSchemes) {
        return MakeCharArray([SBDeviceInfo GetInstalledAppsWithUrlSchemesJSON:CreateNSString(urlSchemes)]);
    }
    
    void _InitializeAppleSearchAd() {
        [SBDebug log:@"Initialize Search Ad was called externally."];
        [SBAppleSearchAd initializeAppleSearchAd];
    }
    
    void _InitializeMixpanelWithToken(char* token) {
        [SBDebug log:@"Initialize mixpanel with token was called externally."];
        [SBMixpanel initializeMixpanelWithToken:CreateNSString(token)];
    }
    
    void _TrackAdjustEvent(char* token) {
        [SBDebug log:@"Track Adjust was called externally"];
        [SBAdjust trackEvent:CreateNSString(token)];
    }
    
    void _TrackMixpanelEvent(char* eventName) {
        [SBDebug log:@"Track mixpanel was called externally"];
        NSString *event = CreateNSString(eventName);
        [SBMixpanel trackEvent:event];
    }
    
    void _TrackMixpanelEventWithProperties(char* eventName, char* properties) {
        [SBDebug log:@"Track mixpanel was called externally"];
        NSString *event = CreateNSString(eventName);
        NSString *jsonData = CreateNSString(properties);
        id data = [SBNativeBindings DeserializeMessageJSON:jsonData];
    
        if ([data isKindOfClass:[NSDictionary class]]) {
            [SBMixpanel trackEvent:event withProperties:data];
        } else if ([data isKindOfClass:[NSArray class]]) {
            // Invalid json data
            [SBDebug log:@"Received a json array, but expected a dictionary."];
        }

    }
    
    void _TimeMixpanelEvent(char* eventName) {
        [SBDebug log:@"Time mixpanel event was called externally"];
        [SBMixpanel timeEvent:CreateNSString(eventName)];
    }

    char* _GetMixpanelDistinctID() {
        return MakeCharArray([SBMixpanel getDistinctID]);
    }
    
    void _SetMixpanelDistinctID(char* distinctID) {
        [SBDebug log:@"Set mixpanel distinct id was called externally."];
        [SBMixpanel setDistinctID:CreateNSString(distinctID)];
    }
    
    void _RegisterMixpanelSuperProperties(char* properties) {
        [SBDebug log:@"Register mixpanel superproperties was called externally."];
        NSString *jsonData = CreateNSString(properties);
        
        id data = [SBNativeBindings DeserializeMessageJSON:jsonData];
        
        if ([data isKindOfClass:[NSDictionary class]]) {
            [SBMixpanel registerSuperProperties:data];
        } else if ([data isKindOfClass:[NSArray class]]) {
            // Invalid json data
            [SBDebug log:@"Received a json array, but expected a dictionary."];
        }
    }
      
    void _SaveEventInfo(char* eventName, char* eventData) {
        [SBDebug log:@"_SaveEventInfo was called externally"];

        NSString *eventNameStr = CreateNSString(eventName);
        
        NSString *jsonData = CreateNSString(eventData);
        id data = [SBNativeBindings DeserializeMessageJSON:jsonData];

        if ([data isKindOfClass:[NSDictionary class]]) {
            if ([eventNameStr isEqualToString:ANALYTICS_EVENT_APP_EXIT]) {
                [[[SBNativeBindings sharedInstance] analyticsAppExit] saveEventInfo:data];
            } else {
                [SBDebug log:[NSString stringWithFormat:@"_SaveEventInfo unsupported event %@", eventNameStr]];
            }
        } else {
            // Invalid json data
            [SBDebug log:@"Invalid eventData JSON"];
        }
    }

}

@end
