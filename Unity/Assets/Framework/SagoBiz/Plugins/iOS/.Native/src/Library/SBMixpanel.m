//
//  SBMixpanel.m
//  SagoBiz
//
//  Created by Aram on 2015-07-02.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import "SBMixpanel.h"
#import "SBDebug.h"

@implementation SBMixpanel

static bool initialized;


+ (void)initializeMixpanelWithToken:(NSString*)token {
    if (!initialized) {
        [Mixpanel sharedInstanceWithToken:token];
        initialized = true;
    } else {
        [SBDebug log:@"Mixpanel is already initialized"];
    }
}

+ (BOOL)isInitialized {
    if (initialized) {
        return YES;
    } else {
        
        [SBDebug log:@"Mixpanel is not initialized."];
        return NO;
    }
}

+ (void)trackEvent:(NSString*)event withProperties:(NSDictionary*)properties {
    if ([SBMixpanel isInitialized]) {
        [SBDebug log:[NSString stringWithFormat:
                      @"Mixpanel - Track Event - Event='%@' Properties='%@'",
                      event,
                      properties]];

        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        [mixpanel track:event properties:properties];
    }
}

+ (void)trackEvent:(NSString*)event {
    if ([SBMixpanel isInitialized]) {
        [SBDebug log:[NSString stringWithFormat:@"Mixpanel - Track Event - Event='%@'", event]];
        
        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        [mixpanel track:event];
        
        [SBDebug log:[NSString stringWithFormat:@"Mixpanel - Current Super Properties:\n%@", [mixpanel currentSuperProperties]]];
    }
}

+ (void)timeEvent:(NSString*)event {
    if ([SBMixpanel isInitialized]) {
        [SBDebug log:[NSString stringWithFormat:@"Mixpanel - Time Event - Event='%@'", event]];
        
        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        [mixpanel timeEvent:event];
        
        [SBDebug log:[NSString stringWithFormat:@"Mixpanel - Current Super Properties:\n%@", [mixpanel currentSuperProperties]]];
    }
}

+ (NSString*)getDistinctID {
    if ([SBMixpanel isInitialized]) {
        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        return [mixpanel distinctId];
    } else {
        return @"";
    }
}

+ (void)setDistinctID:(NSString*)distinctID {
    if ([SBMixpanel isInitialized]) {
        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        [mixpanel identify:distinctID];
    }
}

+ (void)registerSuperProperties:(id)properties {
    if ([SBMixpanel isInitialized]) {
        Mixpanel *mixpanel = [Mixpanel sharedInstance];
        
        if ([properties isKindOfClass:[NSDictionary class]]) {
            [SBDebug log:[NSString stringWithFormat:@"Registering super properties from an internal call:\n%@",  (NSDictionary *)properties]];
            
            [mixpanel registerSuperProperties:properties];
            
            [SBDebug log:[NSString stringWithFormat:@"Modified super property:\n%@", [mixpanel currentSuperProperties]]];
        } else {
            [SBDebug log:@"Super properties are not in the valid json format"];
        }
    }
}

@end
