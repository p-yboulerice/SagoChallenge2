#import "UnityViewControllerBase.h"

#if UNITY_HAS_IOSSDK_11_0 || UNITY_HAS_TVOSSDK_11_0

@implementation UnityViewControllerBase(DeferSystemGestures)


/**
 * On versions before iOS 11, it was assumed that hiding the status bar 
 * means that you want to get first shot at edge gestures. 
 * On iOS 11 and up, this has been made explicit by overriding the method 
 * on UIViewController below. 
 */
- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures
{
    return UIRectEdgeAll;
}

@end

#endif
