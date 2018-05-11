#import "SBAnalyticsAppExit.h"
#import "SBMixpanel.h"

NSString * const ANALYTICS_EVENT_APP_EXIT = @"App Exit";

@interface SBAnalyticsAppExit ()
@property (nonatomic, readwrite, retain) NSMutableDictionary* eventInfo;
@end


@implementation SBAnalyticsAppExit

- (id)init {
    if (self = [super init]) {
        self.eventInfo = [NSMutableDictionary new];

        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(onWillEnterForegroundNotification:)
                                                     name:@"UIApplicationWillEnterForegroundNotification"
                                                   object:nil];
        
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(onDidEnterBackgroundNotification:)
                                                     name:@"UIApplicationDidEnterBackgroundNotification"
                                                   object:nil];
    }

    return self;
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)saveEventInfo:(NSDictionary *)newEventInfo {
    if (newEventInfo) {
        [self.eventInfo addEntriesFromDictionary:newEventInfo];
    }
}

#pragma Notification Handlers

- (void)onWillEnterForegroundNotification:(NSNotification *)notification {
    // When the app is relaunched (transitioning from running in the background
    // to active state) restart the APP_EXIT duration timer to indicate a new
	// user session.
    [SBMixpanel timeEvent:ANALYTICS_EVENT_APP_EXIT];

    // Note: During a fresh app launch (app was not running in the background)
    // we start this timer in Unity (SagoApp -> AnalyticsController -> Init).
}

- (void)onDidEnterBackgroundNotification:(NSNotification *)notification {
    if (self.eventInfo && [self.eventInfo count] >= 1) {
        [SBMixpanel trackEvent:ANALYTICS_EVENT_APP_EXIT withProperties:self.eventInfo];
        
        // Event info has been sent so clear it.
        [self.eventInfo removeAllObjects];
    } else {
        [SBMixpanel trackEvent:ANALYTICS_EVENT_APP_EXIT];
    }
}

@end
