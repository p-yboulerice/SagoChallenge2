//
//  SagoBizDebug.h
//  SagoBiz
//
//  Created by Aram on 2015-08-26.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface SBDebug : NSObject

/*! Enables / disables debug logs  
 \param value the value
 */
+ (void)setEnabled:(BOOL)value;

/*! Gets whether the debug logging is enabled. 
 \returns a BOOL indicating the debug logging status.
 */
+ (BOOL)isEnabled;

/*! Logs a message to debugger window
 \param message The message to be logged.
*/
+ (void)log:(NSString*)message;

@end
