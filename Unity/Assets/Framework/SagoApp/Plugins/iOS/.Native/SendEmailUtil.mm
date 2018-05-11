#import "SendEmailUtil.h"

extern "C" {
    void _SendEmailWithBugReport(char *recipientEmail, char *title, char *body, char *bugReport) {
        [[SendEmailUtil SharedInstance] SendEmailWithBugReport:
            [[NSString alloc] initWithUTF8String: recipientEmail] :
            [[NSString alloc] initWithUTF8String: title] :
            [[NSString alloc] initWithUTF8String: body] :
            [[NSString alloc] initWithUTF8String: bugReport]];
    }
}


@implementation SendEmailUtil

+ (SendEmailUtil*) SharedInstance {
    static SendEmailUtil *SharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        SharedInstance = [[SendEmailUtil alloc] init];
    });
    return SharedInstance;
}

- (void) SendEmailWithBugReport: (NSString*)recipientEmail : (NSString*)title : (NSString*)body : (NSString*)bugReport {
    if([MFMailComposeViewController canSendMail]) {
        NSArray *toRecipents = [NSArray arrayWithObject:recipientEmail];
        NSData *myData = [bugReport dataUsingEncoding:NSUTF8StringEncoding];
        
        MFMailComposeViewController *mailComposer = [[MFMailComposeViewController alloc] init];
        mailComposer.mailComposeDelegate = self;
        [mailComposer setToRecipients:toRecipents];
        [mailComposer setSubject:title];
        [mailComposer setMessageBody:body isHTML:NO];
        [mailComposer addAttachmentData:myData mimeType:@"text/plain" fileName:@"device_info.txt"];
        
        self.modalPresentationStyle = UIModalPresentationOverCurrentContext;
        UIViewController *topController = [UIApplication sharedApplication].keyWindow.rootViewController;
        [topController presentViewController:self animated:NO completion:^{
            [self presentViewController: mailComposer animated:YES completion:NULL];
        }];
    }
    else {
        NSLog(@"The device does not support the Apple mail composer interface or does not have at least one email account enabled on the device.");
    }
}

- (void)mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(nullable NSError *)error {
    [controller dismissViewControllerAnimated:YES completion:nil];
    [self dismissViewControllerAnimated:YES completion:nil];
}
@end
