//
//  SagoBizHelper.m
//  SagoBiz
//
//  Created by Aram on 2015-08-26.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import "SBDebug.h"

@implementation SBDebug

static BOOL _isEnabled;

+ (void)setEnabled:(BOOL)value {
    _isEnabled = value;
}

+ (BOOL)isEnabled {
    return _isEnabled;
}

+ (void)log:(NSString*)message {
    if (_isEnabled) {
        NSLog(@"SagoBizNative-> %@", message);
    }
}

@end
