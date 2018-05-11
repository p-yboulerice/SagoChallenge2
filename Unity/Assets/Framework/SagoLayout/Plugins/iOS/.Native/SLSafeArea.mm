#include <CoreGraphics/CoreGraphics.h>
#include "UnityAppController.h"
#include "UI/UnityView.h"

extern "C" {
    
    void _GetSafeArea(float* x, float* y, float* w, float* h) {
        UIView* view = GetAppController().unityView;
        CGRect area = ComputeSafeArea(view);
        *x = area.origin.x;
        *y = area.origin.y;
        *w = area.size.width;
        *h = area.size.height;
    }

    void _GetSafeAreaInsets(int* x, int* y, int* w, int* h) {
        
        UIView* view = GetAppController().unityView;
        UIEdgeInsets insets = UIEdgeInsetsMake(0, 0, 0, 0);
        #if UNITY_HAS_IOSSDK_11_0 || UNITY_HAS_TVOSSDK_11_0
            if (@available(iOS 11.0, tvOS 11.0, *)) {
                    insets = [view safeAreaInsets];
            }
        #endif
        
        float scale = view.contentScaleFactor;
        insets.left *= scale;
        insets.bottom *= scale;
        insets.right *= scale;
        insets.top *= scale;

        *x = insets.left;
        *y = insets.bottom;
        *w = insets.right;
        *h = insets.top;
    }
}
