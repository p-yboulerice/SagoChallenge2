#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>

extern "C" {
    void _SendEmailWithBugReport(char *recipientEmail, char *title, char *body, char *bugReport);
}

@interface SendEmailUtil : UIViewController<MFMailComposeViewControllerDelegate>

+ (SendEmailUtil*) SharedInstance;
- (void) SendEmailWithBugReport: (NSString*)recipientEmail : (NSString*)title : (NSString*)body : (NSString*)bugReport;

@end
