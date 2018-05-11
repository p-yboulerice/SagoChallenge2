//
//  SAKeychainStore.m
//
//  Created by Minsoo on 2017-04-20.
//  Copyright © 2017 Sago Sago. All rights reserved.
//

#import "SAKeychainStore.h"
#import "UICKeyChainStore.h"

@implementation SAKeychainStore

extern "C" {

    bool _SetStringInKeychain(const char *key, const char *value);

    bool _SetSynchronizedStringInKeychain(const char *key, const char *value);

    const char* _GetStringFromKeychain(const char *key);

    const char* _GetSynchronizedStringFromKeychain(const char *key);

    void _DeleteStringFromKeychain(const char *key);

    void _DeleteSynchronizedStringFromKeychain(const char *key);

    bool _KeychainContainsKey(const char *key, bool synchronized);

}

bool _SetStringInKeychain(const char *key, const char *value) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    NSString *convValue = [NSString stringWithCString:value encoding:NSUTF8StringEncoding];

    return [UICKeyChainStore setString:convValue forKey:convKey];
}

bool _SetSynchronizedStringInKeychain(const char *key, const char *value) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    NSString *convValue = [NSString stringWithCString:value encoding:NSUTF8StringEncoding];

    UICKeyChainStore *uicKeyChainStoreInst = [[UICKeyChainStore alloc] init];
    [uicKeyChainStoreInst setSynchronizable:true];
    return [uicKeyChainStoreInst setString:convValue forKey:convKey];
}

const char* _GetStringFromKeychain(const char *key) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    NSString *retString = [UICKeyChainStore stringForKey:convKey];

    if (retString == nil || [retString isEqualToString:@""]) {
        NSLog(@"No string data for key: %@", convKey);
        retString = @"";
    }

    return makeStringCopy([retString UTF8String]);
}

const char* _GetSynchronizedStringFromKeychain(const char *key) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    UICKeyChainStore *uicKeyChainStoreInst = [[UICKeyChainStore alloc] init];
    [uicKeyChainStoreInst setSynchronizable:true];
    NSString *retString = [uicKeyChainStoreInst stringForKey:convKey];

    if (retString == nil || [retString isEqualToString:@""]) {
        NSLog(@"No string data for key: %@", convKey);
        retString = @"";
    }

    return makeStringCopy([retString UTF8String]);
}

void _DeleteStringFromKeychain(const char *key) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    [UICKeyChainStore removeItemForKey:convKey];
}

void _DeleteSynchronizedStringFromKeychain(const char *key) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    UICKeyChainStore *uicKeyChainStoreInst = [[UICKeyChainStore alloc] init];
    [uicKeyChainStoreInst setSynchronizable:true];
    [uicKeyChainStoreInst removeItemForKey:convKey];
}

bool _KeychainContainsKey(const char *key, bool synchronized) {
    NSString *convKey = [[NSString alloc] initWithUTF8String:key];
    UICKeyChainStore *uicKeyChainStoreInst = [[UICKeyChainStore alloc] init];
    [uicKeyChainStoreInst setSynchronizable:synchronized];
    return [uicKeyChainStoreInst contains:convKey];
}

char* makeStringCopy(const char* str) {
    if (str == NULL) {
        return NULL;
    }

    char* res = (char*)malloc(strlen(str) + 1);
    strcpy(res, str);
    return res;
}

@end
