//
//  SagoMixPanel.h
//  SagoBiz
//
//  Created by Aram on 2015-07-02.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Mixpanel.h"

@interface SBMixpanel : NSObject

/*! Initializes the mixpanel with the provided Token id. It is safe to call this function many times as it handles the redundancy internally.
 \param token The token id for the mixpanel.
 */
+ (void)initializeMixpanelWithToken:(NSString*)token;

/*! Tracks an event with the given properties.
 \param event The event name
 \param properties the key-value dictionary holding the analytics information
 */
+ (void)trackEvent:(NSString*)event withProperties:(NSDictionary*)properties;

/*! Tracks an event with the given properties.
 \param event The event name
 */
+ (void)trackEvent:(NSString*)event;

/*! Starts a timer that will be stopped and added as a property when a corresponding event is tracked.
 \param event The event name
 */
+ (void)timeEvent:(NSString*)event;

/*! Gets the unique identifier for the device. */
+ (NSString*)getDistinctID;

/*! Sets the unique identifier for the device.
 \param distinctID The id that is set as the unique identifier
 */
+ (void)setDistinctID:(NSString*)distinctID;

/*! Registers the properties that will be automatically included in each tracking.
 \param properties The set of key-value information that will be in each track event
 */
+ (void)registerSuperProperties:(id)properties;

@end
