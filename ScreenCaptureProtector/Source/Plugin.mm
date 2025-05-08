//
//  Plugin.m
//  ScreenCaptureProtector
//
//  Created by SeenWonderAlex on 12/25/24.
//

#include "Plugin.pch"


void SetNoSharingToWindow() {
    defaultWindow().sharingType = NSWindowSharingNone;
}

void SetReadSharingToWindow() {
    defaultWindow().sharingType = NSWindowSharingReadOnly;
}

void SetFloatingWindowState(int state)
{
    if (state == 1)
    {
        defaultWindow().level = NSFloatingWindowLevel;
    }
    else
    {
        defaultWindow().level = NSNormalWindowLevel;
    }
}

bool IsWindowSharingAllowed()
{
    return defaultWindow().sharingType != NSWindowSharingNone;
}

NSWindow* defaultWindow() {
    // Find the "best" default window (main, key, or other) that also has a contentViewController.
    // Otherwise, find the "best" one of those that doesn't have a contentViewController.
    if (NSApplication.sharedApplication.mainWindow != nil && NSApplication.sharedApplication.mainWindow.contentViewController != nil) {
        return NSApplication.sharedApplication.mainWindow;
    } else if (NSApplication.sharedApplication.keyWindow != nil &&
               NSApplication.sharedApplication.keyWindow.contentViewController != nil) {
        return NSApplication.sharedApplication.keyWindow;
    } else {
        if (NSApplication.sharedApplication.mainWindow != nil)
        {
            return NSApplication.sharedApplication.mainWindow;
        }
        else if (NSApplication.sharedApplication.keyWindow != nil)
        {
            return NSApplication.sharedApplication.keyWindow;
        }
        return NSApplication.sharedApplication.windows.firstObject;
    }
}
