//
//  ViewController.m
//  MSGraph.MailClient
//
//  Created by Simon Jaeger (@simonjaegr) on 08/03/16.
//  Copyright © 2016 Office 365 Development Patterns and Practises.
//

#import "ViewController.h"
#import "MailClient.h"

@interface ViewController ()
@property (nonatomic, weak) IBOutlet UILabel *label;
@property (nonatomic, weak) IBOutlet UIButton *button;
@property (nonatomic, weak) IBOutlet UIActivityIndicatorView *spinner;
@property (nonatomic) MailClient *mailClient;
@end

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
    self.mailClient = [MailClient new];
}

// Switch to a light status bar style when this view is appearing
- (void)viewWillAppear:(BOOL)animated {
    [UIApplication sharedApplication].statusBarStyle = UIStatusBarStyleLightContent;
}

// Switch to the default status bar style when this view is disappearing
- (void)viewWillDisappear:(BOOL)animated {
    [UIApplication sharedApplication].statusBarStyle = UIStatusBarStyleDefault;
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

// Log messages by printing them on the UILabel.
- (void)logMessage:(NSString *)message {
    // Run the logic on the main thread.
    dispatch_async(dispatch_get_main_queue(), ^{
        self.label.text = message;
    });
}

// Event handler for when the button is tapped.
- (IBAction)sendMe:(id)sender {
    // Trigger UI.
    self.button.hidden = NO;
    [self.spinner startAnimating];
    
    [self.mailClient sendMeWithCallback:^(BOOL success) {
        // Log a final message.
        [self logMessage:success ? @"Check your mailbox!" : @"Oops... something went wrong!"];
        
        // Trigger UI on the main thread.
        dispatch_async(dispatch_get_main_queue(), ^{
            self.button.hidden = NO;
            [self.spinner stopAnimating];
        });
    } logger:^(NSString *message) {
        [self logMessage:message];
    }];
}
@end
