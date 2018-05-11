//
//  SBStoreReviewController.m
//
//  Created by Minsoo on 2017-04-20.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import "SBStoreReviewController.h"
#import <StoreKit/StoreKit.h>

@implementation SBStoreReviewController

extern "C" {

    void _RequestReview();

}

void _RequestReview() {
    if ([SKStoreReviewController class]) {
        [SKStoreReviewController requestReview];
    }
}

@end
