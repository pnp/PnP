//
//  MailClient.h
//  MSGraph.MailClient
//
//  Created by Simon Jaeger (@simonjaegr) on 08/03/16.
//  Copyright © 2016 Office 365 Development Patterns and Practises.
//

#import <Foundation/Foundation.h>
#import <MSGraphService.h>
#import <impl/ADALDependencyResolver.h>
#import <ADAuthenticationResult.h>

@interface MailClient : NSObject
@property (strong, nonatomic) ADALDependencyResolver *resolver;
@property (strong, nonatomic) MSGraphServiceClient *graphClient;
@property (nonatomic, assign) BOOL ready;

// Initialize ADAL (Active Directory Authentication Library) and the Microsoft Graph SDK.
- (id)init;

// Get the complete Microsoft Graph URL (including the version).
- (NSString *)getGraphServiceUrl;

// Let a user sign in, before we can call the Microsoft Graph.
- (void)signInWithCallback:(void(^)(BOOL))callback;

// Get the current signed in user.
- (void)getUserWithCallback:(void(^)(MSGraphServiceUser *))callback;

// Create a recipient object using an email address.
- (MSGraphServiceRecipient *)createRecipientWithAddress:(NSString *)address;

// Create a body object using HTML content.
- (MSGraphServiceItemBody *)createBodyWithContent:(NSString *)content;

// Create a message object that can be used when sending a mail.
- (MSGraphServiceMessage *)createMessageWithSubject:(NSString *)subject
                                               content:(NSString *)content
                                         recipients:(NSArray *)recipients;

// Send a mail using a message object.
- (void)sendMailWithMessage:(MSGraphServiceMessage *)message
                   callback:(void(^)(BOOL))callback;

// Send a mail to the current user.
- (void)sendMeWithCallback:(void(^)(BOOL))callback
                    logger:(void(^)(NSString *))logger;
@end
