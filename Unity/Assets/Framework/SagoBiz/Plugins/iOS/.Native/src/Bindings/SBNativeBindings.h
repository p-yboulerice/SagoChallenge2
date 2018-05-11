//
//  SagoBiz.h
//  SagoBiz
//
//  Created by Aram on 2015-06-11.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

//! Project version number for SagoBiz.
FOUNDATION_EXPORT double SagoBizVersionNumber;

//! Project version string for SagoBiz.
FOUNDATION_EXPORT const unsigned char SagoBizVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <SagoBiz/PublicHeader.h>


@interface SBNativeBindings : NSObject

+ (id)sharedInstance;

@end