//
//  HeyzapAds.h
//  Heyzap
//
//  Created by Daniel Rhodes on 5/30/13.
//  Copyright (c) 2013 Heyzap. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "HZInterstitialAd.h"
#import "HZAdsDelegate.h"
#import "HZLog.h"

#ifndef NS_ENUM
#define NS_ENUM(_type, _name) enum _name : _type _name; enum _name : _type
#endif

typedef NS_ENUM(NSUInteger, HZAdOptions) {
    HZAdOptionsNone = 0 << 0,
    HZAdOptionsDisableAutoPrefetching = 1 << 0,
    HZAdOptionsAdvertiserOnly = 1 << 1
};

@interface HeyzapAds : NSObject

+ (void) startWithAppStoreID: (int) appID andOptions: (HZAdOptions) options;
+ (void) startWithOptions: (HZAdOptions) options; //Only use this method if you are using the Social SDK.
+ (BOOL) isStarted;
+ (void) setDebugLevel:(HZDebugLevel)debugLevel;
+ (void) useDebuggingStrategy:(BOOL)choice;

@end