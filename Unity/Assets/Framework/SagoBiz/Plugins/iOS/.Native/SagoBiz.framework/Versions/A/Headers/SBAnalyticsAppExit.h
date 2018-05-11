#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

FOUNDATION_EXPORT NSString * const ANALYTICS_EVENT_APP_EXIT;

@interface SBAnalyticsAppExit : NSObject

- (void)saveEventInfo:(NSDictionary *)newEventInfo;

@end