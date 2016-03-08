//
//  MailClient.m
//  MSGraph.MailClient
//
//  Created by Simon Jaeger (@simonjaegr) on 08/03/16.
//  Copyright © 2016 Office 365 Development Patterns and Practises.
//

#import "MailClient.h"

@implementation MailClient
NSString *const GraphServiceUrl = @"https://graph.microsoft.com/";
NSString *const GraphServiceVersion = @"v1.0";

// Initialize ADAL (Active Directory Authentication Library) and the Microsoft Graph SDK.
- (id)init {
    self = [super init];
    if (self) {
        self.resolver = [[ADALDependencyResolver alloc] initWithPlist];
        self.graphClient = [[MSGraphServiceClient alloc] initWithUrl:[self getGraphServiceUrl]
                                                  dependencyResolver:self.resolver];
        self.ready = NO;
    }
    return self;
}

// Get the complete Microsoft Graph URL (including the version).
- (NSString *)getGraphServiceUrl {
    return [NSString stringWithFormat:@"%@%@", GraphServiceUrl, GraphServiceVersion];
}

// Let a user sign in, before we can call the Microsoft Graph.
- (void)signInWithCallback:(void (^)(BOOL))callback {
    [self.resolver interactiveLogonWithCallback:^(ADAuthenticationResult *result) {
        self.ready = result.status == AD_SUCCEEDED;
        
        // Call the callback so that we may continue.
        callback(self.ready);
    }];
}

// Get the current signed in user.
- (void)getUserWithCallback:(void (^)(MSGraphServiceUser *))callback {
    // Make sure that the client is ready.
    if (!self.ready || self.graphClient == nil) {
        NSLog(@"The mail client has not been initialized properly");
        
        // Nothing to return.
        callback(nil);
    }
    else {
        // Get the user.
        [self.graphClient.me readWithCallback:^(MSGraphServiceUser *user, MSOrcError *error) {
            // Return the user.
            callback(user);
        }];
    }
}

// Create a recipient object using an email address.
- (MSGraphServiceRecipient *)createRecipientWithAddress:(NSString *)address {
    // Create the email address object.
    MSGraphServiceEmailAddress *emailAddress = [MSGraphServiceEmailAddress new];
    emailAddress.address = address;
    
    // Create and return the recipient object.
    MSGraphServiceRecipient *recipient = [MSGraphServiceRecipient new];
    recipient.emailAddress = emailAddress;
    return recipient;
}

// Create a body object using HTML content.
-(MSGraphServiceItemBody *)createBodyWithContent:(NSString *)content {
    MSGraphServiceItemBody *body = [MSGraphServiceItemBody new];
    
    // Set the content type to HTML.
    body.contentType = MSGraphServiceBodyTypeHtml;
    
    // Set the HTML content.
    body.content = content;
    return body;
}

// Create a message object that can be used when sending a mail.
- (MSGraphServiceMessage *)createMessageWithSubject:(NSString *)subject
                                            content:(NSString *)content
                                         recipients:(NSArray *)recipients {
    MSGraphServiceMessage *message = [MSGraphServiceMessage new];
    
    // Set the message subject.
    message.subject = subject;
    
    // Set the importance of the message (Low, Normal, High).
    message.importance = MSGraphServiceImportanceNormal;
    
    
    // Convert the recipient strings to recipient objects used
    // with the Microsoft Graph.
    NSMutableArray *toRecipients = [NSMutableArray array];
    [recipients enumerateObjectsUsingBlock:^(id  _Nonnull obj, NSUInteger idx, BOOL * _Nonnull stop) {
        [toRecipients addObject:[self createRecipientWithAddress:obj]];
    }];
    
    // Set the recipients of the message.
    message.toRecipients = toRecipients;
    
    // Create and set the body object.
    MSGraphServiceItemBody *body = [self createBodyWithContent:content];
    message.body = body;
    return message;
}

// Send a mail using a message object.
- (void)sendMailWithMessage:(MSGraphServiceMessage *)message
                   callback:(void (^)(BOOL))callback {
    // Make sure that the client is ready.
    if (!self.ready || self.graphClient == nil) {
        NSLog(@"The mail client has not been initialized properly");
        
        // Return failure.
        callback(NO);
    }
    else {
        // Send the mail.
        [self.graphClient.me.operations sendMailWithMessage:message
                                            saveToSentItems:YES
                                                   callback:^(int status, MSOrcError *error) {
            
            // Call the callback.
            callback(status == 0 && error == nil);
        }];
    }
}

// Send a mail to the current user.
- (void)sendMeWithCallback:(void (^)(BOOL))callback
                    logger:(void (^)(NSString *))logger {
    // Let a user sign in.
    logger(@"Signing in...");
    [self signInWithCallback:^(BOOL success) {
        if (!success) {
            callback(NO);
            return;
        }
        
        // Get the current user.
        logger(@"Getting user information...");
        
        [self getUserWithCallback:^(MSGraphServiceUser *user) {
            // Validate the user.
            if (user == nil || user.mail == nil) {
                callback(NO);
                return;
            }
            
            // Create the message content.
            NSMutableString *content = [NSMutableString string];
            [content appendString:@"<strong>Lorem ipsum dolor sit amet</strong>, consectetur adipiscing "];
            [content appendString:@"elit, sed do eiusmod tempor incididunt ut labore et dolore "];
            [content appendString:@"magna aliqua. Ut enim ad minim veniam, quis nostrud "];
            [content appendString:@"exercitation ullamco laboris nisi ut aliquip ex ea commodo "];
            [content appendString:@"consequat. Duis aute irure dolor in reprehenderit in voluptate "];
            [content appendString:@"velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint "];
            [content appendString:@"occaecat cupidatat non proident, sunt in culpa qui officia deserunt "];
            [content appendString:@"mollit anim id est laborum."];
            
            // Create the message.
            MSGraphServiceMessage *message = [self createMessageWithSubject:@"Hello #Office365Dev"
                                                                    content:content
                                                                 recipients:[NSArray arrayWithObject:user.mail]];
            
            // Send the message.
            logger(@"Sending mail...");
            [self sendMailWithMessage:message callback:callback];
        }];
    }];
}
@end
