//
//  WindowApi.m
//  WindowApi
//
//  Created by yoshida on 2022/01/28.
//

#include <stdio.h>
#import <Foundation/Foundation.h>
#import <Cocoa/Cocoa.h>
#import "WindowApi.h"

void background() {
    NSWindow *window = [NSApp orderedWindows][0];
    window.collectionBehavior = NSWindowCollectionBehaviorCanJoinAllSpaces | NSWindowCollectionBehaviorFullScreenAuxiliary;
    window.level = kCGDesktopWindowLevel - 1;
    [window setFrameOrigin:NSMakePoint(0,0)];
}

void hideTitleBar() {
    NSWindow* window = [NSApp orderedWindows][0];
    window.styleMask = NSWindowStyleMaskBorderless | NSWindowStyleMaskResizable;
    window.titlebarAppearsTransparent = false;
    window.titleVisibility = NSWindowTitleHidden;
}

void initializePosition() {
    NSWindow *window = [NSApp orderedWindows][0];
    [window setFrameOrigin:NSMakePoint(0,0)];
}
