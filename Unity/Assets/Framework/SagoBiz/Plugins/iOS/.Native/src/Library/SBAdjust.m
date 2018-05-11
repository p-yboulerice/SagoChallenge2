//
//  SBAdjust.m
//  SBAdjust
//
//  Created by Minsoo on 2017-03-21.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import "SBAdjust.h"

#import <UIKit/UIKit.h>
#import <Adjust.h>
#import <SBMixpanel.h>
#import <SBDebug.h>

@interface SBAdjust() <AdjustDelegate>

@end

@implementation SBAdjust

+ (void)load {
    [self sharedInstance];
}

+ (instancetype)sharedInstance {
    static dispatch_once_t once;
    static id sharedInstance;
    dispatch_once(&once, ^{
        sharedInstance = [[self alloc] init];
    });
    return sharedInstance;
}

+ (void)trackEvent:(NSString *)token {
    ADJEvent *event = [ADJEvent eventWithEventToken:token];
    [Adjust trackEvent:event];
}



- (instancetype)init {
    if (self = [super init]) {
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidFinishLaunching:) name:UIApplicationDidFinishLaunchingNotification object:nil];
    }
    return self;
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)adjustAttributionChanged:(ADJAttribution *)attribution {
    [SBDebug log:[NSString stringWithFormat:@"SBAdjust-> adjustAttributionChanged\n%@", attribution.description]];
    
    NSMutableDictionary *props = [@{} mutableCopy];
    if (attribution.network != nil) props[@"[Adjust]-Network"] = attribution.network;
    if (attribution.campaign != nil) props[@"[Adjust]-Campaign"] = attribution.campaign;
    if (attribution.adgroup != nil)  props[@"[Adjust]-Adgroup"] = attribution.adgroup;
    if (attribution.creative != nil) props[@"[Adjust]-Creative"] = attribution.creative;
    
    // 1. Fire our event so we know this happen.
    [SBMixpanel trackEvent:@"[World] Adjust Attribution" withProperties:[props copy]];
    
    // 2. Register the adjust properties as super property so all future events will have this data.
    [SBMixpanel registerSuperProperties:[props copy]];
}

- (void)applicationDidFinishLaunching:(NSNotification *)notification {
    [SBDebug log:@"SBAdjust-> applicationDidFinishLaunching"];
    
    NSString *filepath = [[NSBundle mainBundle] pathForResource:@"SBAdjust" ofType:@"plist"];
    NSDictionary *config = [NSDictionary dictionaryWithContentsOfFile:filepath];
    
    NSString *adjustToken = [config valueForKey:@"AdjustToken"];
    NSString *environment;
    BOOL debug = [[config valueForKey:@"AdjustDebug"] boolValue];
    if (debug) environment = ADJEnvironmentSandbox;
    else environment = ADJEnvironmentProduction;
    
    ADJConfig *adjustConfig = [ADJConfig configWithAppToken:adjustToken
                                                environment:environment
                                      allowSuppressLogLevel:YES];
    
    // For normal log level, "AdjustLogLevel" value in AdjustConfig.plist should be set to 3.
    [adjustConfig setLogLevel:(ADJLogLevel)[[config valueForKey:@"AdjustLogLevel"] integerValue]];
    
    [adjustConfig setDelegate:self];
    
    [Adjust appDidLaunch:adjustConfig];
}

@end
