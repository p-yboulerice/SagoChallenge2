#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


extern "C" {
    void _AudioSessionUtil_ConfigureAudioSession();
}


@interface AudioSessionUtil : NSObject

+ (void)configureAudioSession;

@end