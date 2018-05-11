//
//  SAOnDemandResourceUtil.m
//
//  Created by Minsoo on 2017-04-20.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import "SAOnDemandResourceUtil.h"

@implementation SAOnDemandResourceUtil

static SALowDiskSpaceWarningCallback lowDiskSpaceWarningCallback;

extern "C" {

    void _CheckResourceAvailability(int queryId, const char* resourceTags[], int tagsCount, SAResourceQueryCallback callback) {
        NSLog(@"SAOnDemandResourceUtil-> _CheckResourceAvailability");
        NSMutableArray* tags = [[NSMutableArray alloc] init];
        for (int i = 0; i < tagsCount; i++) {
            [tags addObject:[NSString stringWithUTF8String:resourceTags[i]]];
        }
        [SAOnDemandResourceUtil checkResourceAvailability:queryId withTag:tags usingBlock:callback];
    }

    void _SetLowDiskSpaceWarningCallback(SALowDiskSpaceWarningCallback callback) {
        if (lowDiskSpaceWarningCallback != NULL) {
            NSLog(@"SAOnDemandResourceUtil-> There is already a callback registered for SALowDiskSpaceWarningCallback.");
            return;
        }

        NSLog(@"SAOnDemandResourceUtil-> _SetLowDiskSpaceWarningCallback");
        lowDiskSpaceWarningCallback = callback;
    }

}

+ (void)checkResourceAvailability:(int)queryId withTag:(NSMutableArray*)resourceTags usingBlock:(SAResourceQueryCallback)callback {
    NSLog(@"SAOnDemandResourceUtil-> checkResourceAvailability withPair usingBlock");
    NSSet *tags = [NSSet setWithArray:resourceTags];
    NSBundleResourceRequest *resourceRequest = [[NSBundleResourceRequest alloc] initWithTags:tags];
    [resourceRequest conditionallyBeginAccessingResourcesWithCompletionHandler:^(BOOL resourcesAvailable) {
        if (callback != NULL) {
            NSLog(@"SAOnDemandResourceUtil-> Resource available: %s", resourcesAvailable ? "TRUE" : "FALSE");
            callback(queryId, resourcesAvailable);
        }
    }];
}

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

- (instancetype)init {
    if (NULL != &NSBundleResourceRequestLowDiskSpaceNotification) {
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(lowDiskSpace:)
                                                     name:NSBundleResourceRequestLowDiskSpaceNotification
                                                   object:nil];
    }
    return self;
}

- (void)lowDiskSpace:(NSNotification*)theNotification {
    NSLog(@"SAOnDemandResourceUtil-> lowDiskSpace received a notification: %@", theNotification);

    // lowDiskSpace selector is not being dispatched from main thread, so it is not safe from Unity's end to invoke this
    // callback directly here. So we instead need to create a block to be dispatched via main queue/thread.
    dispatch_async(dispatch_get_main_queue(), ^{
        NSLog(@"SAOnDemandResourceUtil-> lowDiskSpaceWarningCallback async dispatch");
        if (lowDiskSpaceWarningCallback != NULL) {
            lowDiskSpaceWarningCallback();
        }
    });
}

@end
