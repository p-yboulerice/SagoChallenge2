#import <AVFoundation/AVFoundation.h>
#import "AudioSessionUtil.h"

extern "C" {
    void _AudioSessionUtil_ConfigureAudioSession() {
        [AudioSessionUtil configureAudioSession];
    }
}


@implementation AudioSessionUtil

+ (void)configureAudioSession {
    
    // Use the AVAudioSessionCategoryPlayback / kAudioSessionCategory_MediaPlayback
    // audio category so that sound continues to play regardless of the ring/silent
    // switch setting.
    
    if ([AVAudioSession instancesRespondToSelector:@selector(setCategory:error:)]) {
        NSError *error = nil;
        if ([AVAudioSession.sharedInstance setCategory:AVAudioSessionCategoryPlayback error:&error]) {
            NSLog(@"Successfully set audio session category to AVAudioSessionCategoryPlayback");
        } else {
            NSLog(@"Failed to set audio session category: %@", error);
        }
    } else {
        UInt32 value = kAudioSessionCategory_MediaPlayback;
        OSStatus status = AudioSessionSetProperty(kAudioSessionProperty_AudioCategory, sizeof(value), &value);
        if (status == kAudioSessionNoError) {
            NSLog(@"Successfully set audio session category to kAudioSessionCategory_MediaPlayback");
        } else {
            NSLog(@"Faild to set audio session category: %d", status);
        }
    }
    
}

@end