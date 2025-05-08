#import <UIKit/UIKit.h>
#import "UnityAppController.h"

static UITextField *field = nil;

/***********************************************
Credit to https://stackoverflow.com/a/78620965

This code is licensed under CC BY-SA 4.0
https://creativecommons.org/licenses/by-sa/4.0/
***********************************************/

void addSecureView()
{
    UnityAppController *unityAppController = GetAppController();
    UIView *view = [unityAppController.rootViewController view];
    
    
    if (field == nil) field = [[UITextField alloc] init];
    UIView *secureView = nil;
        
    for (UIView *subview in field.subviews)
    {
        if ([NSStringFromClass([subview class]) containsString:@"TextLayoutCanvasView"])
        {
            secureView = subview;
        }
    }
        
    CALayer *layer = view.layer;
    if (secureView == nil) {
        NSLog(@"NO SECURE VIEW DETECTED - ERROR OCCURED");
        return;
    }
    CALayer *previousLayer = secureView.layer;
    [secureView setValue:layer forKey:@"layer"];
    field.secureTextEntry = YES;
    [secureView setValue:previousLayer forKey:@"layer"];
}

void removeSecureView()
{
    UnityAppController *unityAppController = GetAppController();
    UIView *view = [unityAppController.rootViewController view];
    CALayer *layer = view.layer;
    if (field != nil)
    {
        UIView *secureView = nil;
        for (UIView *subview in field.subviews)
        {
            if ([NSStringFromClass([subview class]) containsString:@"TextLayoutCanvasView"])
            {
                secureView = subview;
            }
        }
        if (secureView == nil) {
            NSLog(@"NO SECURE VIEW DETECTED - ERROR OCCURED");
            return;
        }
        CALayer *previousLayer = secureView.layer;
        [secureView setValue:layer forKey:@"layer"];
        field.secureTextEntry = NO;
        [secureView setValue:previousLayer forKey:@"layer"];
    }
}
