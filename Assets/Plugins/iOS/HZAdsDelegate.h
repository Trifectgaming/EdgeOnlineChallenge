//
//  HZAdsDelegate.h
//  Heyzap
//
//  Created by David Stumm on 1/30/13.
//
//

#import <Foundation/Foundation.h>

/** The `HZAdsDelegate` protocol provides global information about our ads. If you want to know if we had an ad to show after calling `showAd` (to e.g. fallback to another ads provider), I highly recommend using the `showAd:completion:` method instead. */
@protocol HZAdsDelegate <NSObject>

@optional

#pragma mark - Showing ads callbacks

/** Called when we succesfully show an ad. */
- (void)didShowAdWithTag: (NSString *) tag;

/** Called when `showAd` (or a variant) is called but we don't have an ad to show. Because we prefetch ads, this should be a rare occurence.
 @param error An `NSError` whose `userInfo` dictionary contains a description of the problem inside the `NSLocalizedDescriptionKey` key.
 */
- (void)didFailToShowAdWithTag: (NSString *) tag andError: (NSError *)error;

/** Called when we receive a valid ad from our server. */
- (void)didReceiveAdWithTag: (NSString *) tag;
/** Called when our server fails to send a valid ad. This should be a rare occurence; only when our server returns invalid data or has a 500 error, etc. */
- (void)didFailToReceiveAdWithTag: (NSString *) tag;

/** Called when the user clicks on an ad. */
- (void)didClickAdWithTag: (NSString *) tag;
/** Called when the ad is dismissed. */
- (void)didHideAdWithTag: (NSString *) tag;

@end
