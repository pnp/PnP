/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

typedef NS_ENUM(NSInteger, MSOrcHttpVerb) {
    
    HTTP_VERB_GET,
    HTTP_VERB_POST,
    HTTP_VERB_DELETE,
    HTTP_VERB_PUT,
    HTTP_VERB_HEAD,
    HTTP_VERB_OPTIONS,
    HTTP_VERB_PATCH
};