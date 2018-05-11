//
//  SagoDeviceInfo.h
//  SagoBiz
//
//  Created by Aram on 2015-06-15.
//  Copyright (c) 2015 Sago Sago Toys Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface SBDeviceInfo : NSObject

/*! Gets the the current app's bundle identifier. */
+ (NSString*)BundleId;

/*! Gets the current app's bundle version (CFBundleShortVersionString). */
+ (NSString*)BundleVersion;

/*! Gets the current app's bundle version code. */
+ (NSString*)BundleVersionCode;

/*! Gets the current app's name. */
+ (NSString *)AppName;

/*! Gets the preferred language name of the device. */
+ (NSString*)Language;

/*! Gets the language code in ISO 639-1. This is a 2-letter code in lowercase. */
+ (NSString*)LanguageCode;

/*! Gets the country code in ISO 639-1. This is a 2-letter code in uppercase. */
+ (NSString*)CountryCode;

/*! Gets the operating system name. */
+ (NSString*)OperatingSystem;

/*! Gets the operating system version. */
+ (NSString*)OperatingSystemVersion;

/*! Gets whether the device has a camera. */
+ (BOOL)HasCamera;

/*! Gets the timezone abbreviation. */
+ (NSString*)TimeZone;

/*! Gets the current app's default url scheme. The format is 'bundleId://' without the quotes. */
+ (NSString*)UrlScheme;

/*! Gets the alphanumeric string that uniquely identifies a device to the app’s vendor. */
+ (NSString*)VendorId;

/*! Check whether the given apps are installed on the device.
 \param urlSchemesJSON The bundle id dictionary that is used to check the url schemes.
 Format: {'bundle_id':'com.companyname.appname'}
 */
+ (NSString*)GetInstalledAppsWithUrlSchemesJSON:(NSString*)urlSchemesJSON;

@end
