//
//  HZInterstitialAd.h
//  Heyzap
//
//  Created by Daniel Rhodes on 5/31/13.
//  Copyright (c) 2013 Heyzap. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "HZAdsDelegate.h"

/** Controller class for showing Heyzap's Interstitial Ads. Typical users will only need to call the `show` method. */
@interface HZInterstitialAd : NSObject

#pragma mark - Showing Ads

/** Shows an interstitial ad. */
+ (void) show;

/** Shows an interstitial ad.
 
 @param tag A string identifying the context in which the ad was shown, e.g. "After level 1". In the future, Heyzap will breakdown ads data based on this value. */
+ (void) showForTag: (NSString *) tag;

/** Shows an interstitial ad.
 
 @param tag A string identifying the context in which the ad was shown, e.g. "After level 1". In the future, Heyzap will breakdown ads data based on this value.
 @param completion Completion block
 `result` `YES` if there was an ad to show, otherwise `NO`.
 `error` `nil` if there was an ad to show. Otherwise, the error's `userInfo` dictionary will contain an explanation under the `NSLocalizedDescriptionKey`.
 */
+ (void) showForTag:(NSString *)tag completion:(void (^)(BOOL result, NSError *error))completion;

#pragma mark - Callbacks

/** Sets the delegate to receive the messages listed in the `HZAdsDelegate` protocol.
 
 @param delegate The object to receive the callbacks. 
 */
+ (void) setDelegate: (id<HZAdsDelegate>) delegate;

#pragma mark - Manual Control of Ads

// Typical users of the SDK won't need to call these methods. However, you may use them to achieve more fine-tuned control, especially if you are using multiple ad networks and want to minimize unnecessary server requests.

/** Fetches a new ad from Heyzap.  */
+ (void) fetch;

/** Fetches a new ad from Heyzap with an optional completion handler */
+ (void) fetchWithCompletion: (void (^)(BOOL result, NSError *error))completion;

/** Fetches a new ad for a tag from Heyzap */
+ (void) fetchForTag: (NSString *) tag;

/** Fetches a new ad for a tag from Heyzap with an optional completion handler */
+ (void) fetchForTag:(NSString *)tag withCompletion: (void (^)(BOOL result, NSError *error))completion;

/** Whether or not an ad is available to show. */
+ (BOOL) isAvailable;

/** Whether or not an ad with the particular tag is available to show. */
+ (BOOL) isAvailableForTag: (NSString *) tag;

/** Dismisses the current ad, if visible. If an ad has been fetched from the server, clears all data about that ad from memory. If auto-prefetching has not been turned off, this method also fetches a new ad. */
+ (void) hide;



#pragma mark - Private methods

/** Private method
 @param creativeID private
 */
+ (void)showAdWithCreativeID:(NSString *)creativeID;

/** Private method
 @param options private
 */
+ (void)showAdWithOptions:(NSDictionary *)options;

@end
