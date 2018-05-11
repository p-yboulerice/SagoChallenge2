//
//  SagoDeviceInfo.m
//  SagoBiz
//
//  Created by Aram on 2015-06-15.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import "SBDeviceInfo.h"
#import "SBDebug.h"


@implementation SBDeviceInfo

+ (NSString*)BundleId {
    
    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    
    return bundleIdentifier;
}

+ (NSString*)BundleVersion {

    NSString *result = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleShortVersionString"];
    if ([result length] == 0)
        result = @"";
    
    return result;
}

+ (NSString*)BundleVersionCode {
    
    NSString *result = [[NSBundle mainBundle] objectForInfoDictionaryKey:(NSString*)kCFBundleVersionKey];
    if ([result length] == 0)
        result = @"";
    
    return result;
}

+ (NSString *)AppName {
    NSString *localizedName = [[[NSBundle mainBundle] localizedInfoDictionary] objectForKey:@"CFBundleDisplayName"];
    NSString *appName = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleDisplayName"];
    return localizedName ? localizedName : appName;
}

+ (NSString*)Language {
    return [[NSLocale preferredLanguages] objectAtIndex:0];
}

+ (NSString*)LanguageCode {
    return [[NSLocale currentLocale] objectForKey: NSLocaleLanguageCode];
}

+ (NSString*)CountryCode {
    return [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
}

+ (NSString*)OperatingSystem {
    return [[UIDevice currentDevice] systemName];
}

+ (NSString*)OperatingSystemVersion {
    return [[UIDevice currentDevice] systemVersion];
}

+ (BOOL)HasCamera {
    return [UIImagePickerController isSourceTypeAvailable: UIImagePickerControllerSourceTypeCamera];
}

+ (NSString*)TimeZone {
    return [[NSTimeZone localTimeZone] abbreviation];
}

+ (NSString*)UrlScheme {
    return [[SBDeviceInfo BundleId] stringByAppendingString:@"://"];
}

+ (NSString*)VendorId {
 
    NSString *identifier = nil;
    if (NSClassFromString(@"UIDevice") && [[UIDevice currentDevice] respondsToSelector:@selector(identifierForVendor)]) {
        identifier = [[UIDevice currentDevice].identifierForVendor UUIDString];
    }
    
    return identifier;
}

+ (NSString*)GetInstalledAppsWithUrlSchemesJSON:(NSString*)urlSchemesJSON {
    
    id data = [SBDeviceInfo DeserializeMessageJSON:urlSchemesJSON];

    if ([data isKindOfClass:[NSDictionary class]]) {
        [SBDebug log:@"Received a dictionary, but expected a json array."];
        return @"[{ \"bundle_id\": \"invalid\", \"installed\": false }]";
        
    } else if ([data isKindOfClass:[NSArray class]]) {
        
        NSMutableArray *result = [NSMutableArray new];
        
        for (NSDictionary *url in data) {
            if ([url valueForKey:@"bundle_id"] != nil) {
                NSString *urlScheme = [[url valueForKey:@"url_scheme"] stringByAppendingString:@"://"];
                BOOL available = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:urlScheme]];
                
                NSDictionary *jsonDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                                [url valueForKey:@"bundle_id"], @"bundle_id",
                                                [url valueForKey:@"name"], @"name",
                                                available ? @"true" : @"false", @"installed",
                                                [url valueForKey:@"url_scheme"], @"url_scheme",
                                                [url valueForKey:@"company_name"], @"company_name",
                                                nil];
                [result addObject:jsonDictionary];
            }
        }
        
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:result options:NSJSONWritingPrettyPrinted error:nil];
        NSString *ret = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return ret;
        
    } else {
        [SBDebug log:@"Received string or invalid data"];
        return @"[{ \"bundle_id\": \"invalid\", \"installed\": false }]";
    }
    
}

+ (NSArray*)DeserializeMessageJSON:(NSString *)messageJSON {
    return [NSJSONSerialization JSONObjectWithData:[messageJSON dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:nil];
}

@end
