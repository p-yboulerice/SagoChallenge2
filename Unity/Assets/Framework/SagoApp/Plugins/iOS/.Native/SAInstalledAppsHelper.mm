//
//  SAInstalledAppsHelper.m
//
//  Created by Minsoo on 2017-04-20.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import "SAInstalledAppsHelper.h"
#import "SAKeychainStore.h"

@implementation SAInstalledAppsHelper

extern "C" {

    char* _GetInstalledApps(char *appCodeJSON);

}

char* _GetInstalledApps(char *appCodeJSON) {

    id data = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:appCodeJSON] dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:nil];

    if ([data isKindOfClass:[NSDictionary class]]) {
        NSLog(@"Received a dictionary, but expected a json array.");
        return strdup([@"[{ \"url_scheme\": \"invalid\", \"installed\": false }]" UTF8String]);

    } else if ([data isKindOfClass:[NSArray class]]) {

        NSMutableArray *result = [NSMutableArray new];

        for (NSDictionary *url in data) {
            if ([url valueForKey:@"url_scheme"] != nil) {
                NSString *urlScheme = [[url valueForKey:@"url_scheme"] stringByAppendingString:@"://"];
                BOOL available = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:urlScheme]];

                NSDictionary *jsonDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                                [url valueForKey:@"url_scheme"], @"url_scheme",
                                                available ? @"true" : @"false", @"installed",
                                                nil];
                [result addObject:jsonDictionary];
            }
        }

        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:nil];
        NSString *ret = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return strdup([ret UTF8String]);

    } else {
        NSLog(@"Received string or invalid data");
        return strdup([@"[{ \"url_scheme\": \"invalid\", \"installed\": false }]" UTF8String]);
    }

}

@end
