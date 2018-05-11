//
//  SAOnDemandResourceUtil.h
//
//  Created by Minsoo on 2017-04-20.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import <Foundation/Foundation.h>

#ifndef SAOnDemandResourceUtil_h
#define SAOnDemandResourceUtil_h

@interface SAOnDemandResourceUtil : NSObject

extern "C" {
    typedef void (*SAResourceQueryCallback)(int, BOOL);
    typedef void (*SALowDiskSpaceWarningCallback)();
}

+ (void)checkResourceAvailability:(int)queryId withTag:(NSMutableArray*)resourceTags usingBlock:(SAResourceQueryCallback)callback;

- (void)lowDiskSpace:(NSNotification*)theNotification;

@end

#endif /* SAOnDemandResourceUtil_h */
