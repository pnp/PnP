#Microsoft Azure Active Directory Authentication Library (ADAL) for iOS and OSX
=====================================

[![Build Status](https://travis-ci.org/MSOpenTech/azure-activedirectory-library-for-ios.png)](https://travis-ci.org/MSOpenTech/azure-activedirectory-library-for-ios)
[![Coverage Status](https://coveralls.io/repos/MSOpenTech/azure-activedirectory-library-for-ios/badge.png?branch=master)](https://coveralls.io/r/MSOpenTech/azure-activedirectory-library-for-ios?branch=master)

The ADAL SDK for iOS gives you the ability to add support for Work Accounts to your application with just a few lines of additional code. This SDK gives your application the full functionality of Microsoft Azure AD, including industry standard protocol support for OAuth2, Web API integration with user level consent, and two factor authentication support. Best of all, it’s FOSS (Free and Open Source Software) so that you can participate in the development process as we build these libraries. 

**What is a Work Account?**

A Work Account is an identity you use to get work done no matter if at your business or on a college campus. Anywhere you need to get access to your work life you'll use a Work Account. The Work Account can be tied to an Active Directory server running in your datacenter or live completely in the cloud like when you use Office365. A Work Account will be how your users know that they are accessing their important documents and data backed my Microsoft security.

## ADAL for iOS 1.0 Released!

Thanks to your feedback, we have released the 1.0.0 version of iOS for ADAL  [You can grab the release here] (https://github.com/AzureAD/azure-activedirectory-library-for-objc/releases/tag/1.0.1)

## Samples and Documentation

[We provide a full suite of sample applications and documentation on GitHub](https://github.com/AzureADSamples) to help you get started with learning the Azure Identity system. This includes tutorials for native clients such as Windows, Windows Phone, iOS, OSX, Android, and Linux. We also provide full walkthroughs for authentication flows such as OAuth2, OpenID Connect, Graph API, and other awesome features. 

Visit your Azure Identity samples for iOS is here: [https://github.com/AzureADSamples/NativeClient-iOS](https://github.com/AzureADSamples/NativeClient-iOS)

## Community Help and Support

We leverage [Stack Overflow](http://stackoverflow.com/) to work with the community on supporting Azure Active Directory and its SDKs, including this one! We highly recommend you ask your questions on Stack Overflow (we're all on there!) Also browser existing issues to see if someone has had your question before. 

We recommend you use the "adal" tag so we can see it! Here is the latest Q&A on Stack Overflow for ADAL: [http://stackoverflow.com/questions/tagged/adal](http://stackoverflow.com/questions/tagged/adal)

## Contributing

All code is licensed under the Apache 2.0 license and we triage actively on GitHub. We enthusiastically welcome contributions and feedback. You can clone the repo and start contributing now. 

## Quick Start

1. Clone the repository to your machine
2. Build the library
3. Add the ADALiOS library to your project
4. Add the storyboards from the ADALiOSBundle to your project resources
5. Add libADALiOS to “Link With Libraries” phase. 


##Download

We've made it easy for you to have multiple options to use this library in your iOS project:

###Option 1: Source Zip

To download a copy of the source code, click "Download ZIP" on the right side of the page or click [here](https://github.com/AzureAD/azure-activedirectory-library-for-objc/archive/1.0.0.tar.gz).

###Option 2: Cocoapods

    pod 'ADALiOS', '~> 1.0.2'

## Usage

### ADAuthenticationContext

The starting point for the API is in ADAuthenticationContext.h header. ADAuthenticationContext is the main class used for obtaining, caching and supplying access tokens.

#### How to quickly get a token from the SDK:

```Objective-C
	ADAuthenticationContext* authContext;
	NSString* authority;
	NSString* redirectUriString;
	NSString* resourceId;
	NSString* clientId;

+(void) getToken : (BOOL) clearCache completionHandler:(void (^) (NSString*))completionBlock;
{
    ADAuthenticationError *error;
    authContext = [ADAuthenticationContext authenticationContextWithAuthority:authority
                                                                        error:&error];
    
    NSURL *redirectUri = [NSURL URLWithString:redirectUriString];
    
    if(clearCache){
        [authContext.tokenCacheStore removeAll];
    }
    
    [authContext acquireTokenWithResource:resourceId
                                 clientId:clientId
                              redirectUri:redirectUri
                          completionBlock:^(ADAuthenticationResult *result) {
        if (AD_SUCCEEDED != result.status){
            // display error on the screen
            [self showError:result.error.errorDetails];
        }
        else{
            completionBlock(result.accessToken);
        }
    }];
}
```

#### Adding the Token to the authHeader to access APIs:

```Objective-C

	+(NSArray*) getTodoList:(id)delegate
	{
    __block NSMutableArray *scenarioList = nil;
    
    [self getToken:YES completionHandler:^(NSString* accessToken){
    
    NSURL *todoRestApiURL = [[NSURL alloc]initWithString:todoRestApiUrlString];
            
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc]initWithURL:todoRestApiURL];
            
    NSString *authHeader = [NSString stringWithFormat:@"Bearer %@", accessToken];
            
    [request addValue:authHeader forHTTPHeaderField:@"Authorization"];
            
    NSOperationQueue *queue = [[NSOperationQueue alloc]init];
            
    [NSURLConnection sendAsynchronousRequest:request queue:queue completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
                
            if (error == nil){
                    
            NSArray *scenarios = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
                
            todoList = [[NSMutableArray alloc]init];
                    
            //each object is a key value pair
            NSDictionary *keyVauePairs;
                    
            for(int i =0; i < todo.count; i++)
            {
                keyVauePairs = [todo objectAtIndex:i];
                        
                Task *s = [[Task alloc]init];
                        
                s.id = (NSInteger)[keyVauePairs objectForKey:@"TaskId"];
                s.description = [keyVauePairs objectForKey:@"TaskDescr"];
                
                [todoList addObject:s];
                
             }
                
            }
        
        [delegate updateTodoList:TodoList];
        
        }];
        
    }];
    return nil; } 
```

### Diagnostics

The following are the primary sources of information for diagnosing issues:

+ NSError
+ Logs
+ Network traces

Also, note that correlation IDs are central to the diagnostics in the library. You can set your correlation IDs on a per request basis if you want to correlate an ADAL request with other operations in your code. If you don't set a correlations id then ADAL will generate a random one and all log messages and network calls will be stamped with the correlation id. The self generated id changes on each request.

#### NSError

This is obviously the first diagnostic. We try to provide helpful error messages. If you find one that is not helpful please file an issue and let us know. Please also provide device information such as model and SDK#. The error message is returned as a part of the ADAuthenticationResult where the status is set to AD_FAILED.

#### Logs

You can configure the library to generate log messages that you can use to help diagnose issues. ADAL uses NSLog by default to log the messages. Each API method call is decorated with API version and every other message is decorated with correlation id and UTC timestamp. This data is important to look of server side diagnostics. SDK also exposes the ability to provide a custom Logger callback as follows.
```Objective-C
    [ADLogger setLogCallBack:^(ADAL_LOG_LEVEL logLevel, NSString *message, NSString *additionalInformation, NSInteger errorCode) {
        //HANDLE LOG MESSAGE HERE
    }]
```

##### Logging Levels
+ No_Log(Disable all logging)
+ Error(Exceptions. Set as default)
+ Warn(Warning)
+ Info(Information purposes)
+ Verbose(More details)

You set the log level like this:
```Objective-C
[ADLogger setLevel:ADAL_LOG_LEVEL_INFO]
 ```
 
#### Network Traces

You can use various tools to capture the HTTP traffic that ADAL generates.  This is most useful if you are familiar with the OAuth protocol or if you need to provide diagnostic information to Microsoft or other support channels.

Charles is the easiest HTTP tracing tool in OSX.  Use the following links to setup it up to correctly record ADAL network traffic.  In order to be useful it is necessary to configure Charles, to record unencrypted SSL traffic.  NOTE: Traces generated in this way may contain highly privileged information such as access tokens, usernames and passwords.  If you are using production accounts, do not share these traces with 3rd parties.  If you need to supply a trace to someone in order to get support, reproduce the issue with a temporary account with usernames and passwords that you don't mind sharing.

+ [Setting Up SSL For iOS Simulator or Devices](http://www.charlesproxy.com/documentation/faqs/ssl-connections-from-within-iphone-applications/)



##Common problems

**Application, using the ADAL library crashes with the following exception:**<br/> *** Terminating app due to uncaught exception 'NSInvalidArgumentException', reason: '+[NSString isStringNilOrBlank:]: unrecognized selector sent to class 0x13dc800'<br/>
**Solution:** Make sure that you add the -ObjC flag to "Other Linker Flags" build setting of the application. For more information, see Apple documentation for using static libraries:<br/> https://developer.apple.com/library/ios/technotes/iOSStaticLibraries/Articles/configuration.html#//apple_ref/doc/uid/TP40012554-CH3-SW1.

## License

Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. Licensed under the Apache License, Version 2.0 (the "License"); 
