//
//  ViewController.h
//  SagoPromoTest
//

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import "SBMixpanel.h"

@interface SBWebViewController : UIViewController <UIWebViewDelegate, UIAlertViewDelegate>

@property (nonatomic) BOOL isModal;
@property (nonatomic, readwrite, copy) void (^deallocateBlock)(void);
- (void)OpenURL:(NSString*)url;
@end

