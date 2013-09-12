//
//  HZLog.h
//  Heyzap
//
//

#import <Foundation/Foundation.h>

typedef enum {
    HZDebugLevelVerbose = 3,
    HZDebugLevelInfo = 2,
    HZDebugLevelError = 1,
    HZDebugLevelSilent = 0
} HZDebugLevel;

@interface HZLog : NSObject

+ (void) info: (NSString *) message;
+ (void) error: (NSString *) message;
+ (void) debug: (NSString *) message;
+ (void) setDebugLevel: (HZDebugLevel) debugLevel;


@end
