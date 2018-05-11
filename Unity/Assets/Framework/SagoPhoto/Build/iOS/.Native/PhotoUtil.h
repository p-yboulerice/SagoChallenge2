#import <Foundation/Foundation.h>

@interface PhotoUtil : NSObject

+ (void)savePhotoAtPath:(NSString *)path deleteAfter:(BOOL)deleteAfter;

@end