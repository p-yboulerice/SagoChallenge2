//
//  SBAppleSearchAd.m
//  SagoBiz
//
//  Created by Minsoo on 2017-03-30.
//
//

#import "SBAppleSearchAd.h"
#import "SBMixpanel.h"
#import "SBDebug.h"

@interface SBAppleSearchAd()
+ (void)requestSearchAdAttributionDetailsWithTimeBetweenRetries:(NSTimeInterval)timeBetweenRetries numberOfRetries:(NSInteger)numberOfRetries successBlock:(void(^)(NSDictionary *))successBlock failureBlock:(void(^)(void))failureBlock;
@end

typedef void(^SBAppleSearchAdTimerHandlerSuccessBlock)(NSDictionary *);
typedef void(^SBAppleSearchAdTimerHandlerFailureBlock)(void);

@interface SBAppleSearchAdTimerHandler : NSObject

@property (nonatomic,assign) NSTimeInterval timeBetweenRetries;
@property (nonatomic,assign) NSInteger numberOfRetries;
@property (nonatomic,strong) SBAppleSearchAdTimerHandlerSuccessBlock successBlock;
@property (nonatomic,strong) SBAppleSearchAdTimerHandlerFailureBlock failureBlock;

@end

@implementation SBAppleSearchAdTimerHandler

-(void)retryTimer:(NSTimer *)timer{
    [SBDebug log:@"Retrying to request for Search Ad attribution details."];
    [SBAppleSearchAd requestSearchAdAttributionDetailsWithTimeBetweenRetries:self.timeBetweenRetries numberOfRetries:self.numberOfRetries successBlock:self.successBlock failureBlock:self.failureBlock];
    self.successBlock = nil;
    self.failureBlock = nil;//so that obj does not retain success and failure blocks
}

@end

@implementation SBAppleSearchAd

NSString *const HasReceivedSearchAdAttributionKey = @"SBHasReceivedAppleSearchAdAttibution";

+ (void)initializeAppleSearchAd {
    NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
    // If we haven't already registered HasReceivedSearchAdAttributionKey bool value to NSUserDefaults.
    if ([userDefaults objectForKey:HasReceivedSearchAdAttributionKey] == nil) {
        // Register HasReceivedSearchAdAttributionKey bool value so that we don't make more than one Search Ad attribution request per lifetime of app install.
        [userDefaults setValue:[NSNumber numberWithBool:YES] forKey:HasReceivedSearchAdAttributionKey];
        
        [SBDebug log:@"SBAppleSearchAd-> initializeSearchAd"];
        
        [SBAppleSearchAd requestSearchAdAttributionDetailsWithTimeBetweenRetries:5 numberOfRetries:3 successBlock:^(NSDictionary *attributionDetails) {
            [SBDebug log:@"Successfully got attribution dictionary for search ads."];
            NSDictionary *actualData = attributionDetails[@"Version3.1"];
            int line = __LINE__;
            NSMutableDictionary *d = nil;
            if(actualData) {
                d = [NSMutableDictionary dictionaryWithDictionary:actualData];
                d[@"Attribution Dictionary Version"] = @"3.1";
            }
            else {
                NSLog(@"*****************************************");
                NSLog(@"IMPORTANT:");
                NSLog(@"APPLE CHANGED THE VERSION INFO DICTIONARY FOR SEARCH ADS");
                NSLog(@"PLEASE TAKE A LOOK AT THIS CODE AND UPDATE IT");
                NSLog(@"file:%s line:%d",__FILE__,line);
                NSLog(@"*****************************************");
                
                if(attributionDetails.count) {
                    NSString *firstKey = [[attributionDetails allKeys]firstObject];
                    if([actualData = [attributionDetails objectForKey:firstKey] isKindOfClass:[NSDictionary     class]]){
                        d = [NSMutableDictionary dictionaryWithDictionary:actualData];
                        d[@"Attribution Dictionary Version"] = firstKey;
                    }
                    else {
                        d = [NSMutableDictionary dictionary];
                        d[@"Attribution Dictionary Version"] = @"Unknown";
                        for(NSString *key in attributionDetails) {
                            [d setObject:[attributionDetails[key]description] forKey:key];
                        }
                    }
                }
            }
            
            d[@"App Id"] = [[NSBundle mainBundle]bundleIdentifier];
            NSString *appName = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleName"];
            d[@"App Name"] = appName;
            [SBMixpanel trackEvent:@"[World] iOS Search Ad Info Received" withProperties:(NSDictionary*)d];
            
        } failureBlock:^{
            [SBDebug log:@"Failed to get attribution dictionary for search ads"];
        }];
    }
}

+ (void)requestSearchAdAttributionDetailsWithTimeBetweenRetries:(NSTimeInterval)timeBetweenRetries numberOfRetries:(NSInteger)numberOfRetries successBlock:(void(^)(NSDictionary *))successBlock failureBlock:(void(^)(void))failureBlock {
    [SBDebug log:@"SBAppleSearchAd-> requestSearchAdAttributionDetailsWithTimeBetweenRetries"];
    Class adClient = NSClassFromString(@"ADClient");
    if(adClient != Nil) {
        if ([[ADClient sharedClient] respondsToSelector:@selector(requestAttributionDetailsWithBlock:)]) {
            [[ADClient sharedClient] requestAttributionDetailsWithBlock:^(NSDictionary *attributionDetails, NSError *error) {
                if (error) {
                    switch(error.code) {
                        case ADClientErrorUnknown:
                        {
                            dispatch_async(dispatch_get_main_queue(), ^{
                                if(numberOfRetries > 0) {
                                    SBAppleSearchAdTimerHandler *obj = [[SBAppleSearchAdTimerHandler alloc]init];
                                    obj.numberOfRetries = numberOfRetries - 1;
                                    obj.timeBetweenRetries = timeBetweenRetries;
                                    obj.failureBlock = failureBlock;
                                    obj.successBlock = successBlock;
                                    [NSTimer scheduledTimerWithTimeInterval:timeBetweenRetries target:obj selector:@selector(retryTimer:) userInfo:nil repeats:NO];
                                    obj = nil;//so that the block does not retain obj
                                }
                                else {
                                    dispatch_async(dispatch_get_main_queue(), ^{
                                        failureBlock();
                                    });
                                }
                            });
                        }
                            break;
                        case ADClientErrorLimitAdTracking:
                            dispatch_async(dispatch_get_main_queue(), ^{
                                failureBlock();
                            });
                            break;
                    }
                }
                else {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        successBlock(attributionDetails);
                    });
                }
            }];
        }
        else {
            [SBDebug log:@"iOS 10 ADClient call does not exist"];
        }
    }
    else {
        [SBDebug log:@"ADClient Not Available"];
    }
}

@end
