#import <UIKit/UIKit.h>

@interface UIAlertView (Additions)

+ (void)presentCredentialAlert:(void(^)(NSUInteger index))handler;

+ (id) getAlertInstance;
@end