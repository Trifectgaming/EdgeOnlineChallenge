//
//  HeyzapUnitySDK.m
//
//  Copyright 2013 Smart Balloon, Inc. All Rights Reserved
//
//  Permission is hereby granted, free of charge, to any person
//  obtaining a copy of this software and associated documentation
//  files (the "Software"), to deal in the Software without
//  restriction, including without limitation the rights to use,
//  copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following
//  conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//  OTHER DEALINGS IN THE SOFTWARE.
//

#import "HeyzapAds.h"

extern void UnitySendMessage(const char *, const char *, const char *);

@interface HeyzapUnityAdDelegate : NSObject<HZAdsDelegate>
@end

@implementation HeyzapUnityAdDelegate

- (void) didReceiveAdWithTag:(NSString *)tag {
    NSLog(@"Receive Ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"available", tag] UTF8String]);
}

- (void) didFailToReceiveAdWithTag:(NSString *)tag {
    NSLog(@"Failed to receive ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"fetch_failed", tag] UTF8String]);
}

- (void) didShowAdWithTag:(NSString *)tag {
    NSLog(@"Show Ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"show", tag] UTF8String]);
}

- (void) didHideAdWithTag:(NSString *)tag {
    NSLog(@"Hide ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"hide", tag] UTF8String]);
}

- (void) didFailToShowAdWithTag:(NSString *)tag andError:(NSError *)error {
    NSLog(@"Failed to show ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"failed", tag] UTF8String]);
}

- (void) didClickAdWithTag:(NSString *)tag {
    NSLog(@"Click ad");
    UnitySendMessage("HeyzapAds", "setDisplayState", [[NSString stringWithFormat: @"%@,%@", @"click", tag] UTF8String]);
}

@end

extern "C" {
    void hz_ads_start(int flags) {
        HeyzapUnityAdDelegate *delegate = [[HeyzapUnityAdDelegate alloc] init];
        [HeyzapAds startWithOptions: (HZAdOptions)flags];
        [HZInterstitialAd setDelegate: delegate];
    }
    
     void hz_ads_start_app(int flags, int appStoreId) {
        HeyzapUnityAdDelegate *delegate = [[HeyzapUnityAdDelegate alloc] init];
        [HeyzapAds startWithAppStoreID: appStoreId andOptions: (HZAdOptions)flags];
        [HZInterstitialAd setDelegate: delegate];

     }
     
     void hz_ads_show_interstitial(const char *tag) {
         [HZInterstitialAd showWithTag: [NSString stringWithUTF8String: tag]];
     }
     
     void hz_ads_hide_interstitial(void) {
         [HZInterstitialAd hide];
     }
     
     void hz_ads_fetch_interstitial(const char *tag) {
         [HZInterstitialAd fetchForTag: [NSString stringWithUTF8String: tag]];
     }
     
     bool hz_ads_interstitial_is_available(const char *tag) {
         return [HZInterstitialAd isAvailableForTag: [NSString stringWithUTF8String: tag]];
     }
     
     void hz_ads_show_banner(const char *tag) {
        // Not Implemented
     }
     
     void hz_ads_hide_banner(void) {
        // Not implemented
     }
}