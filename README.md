# Unity Game's Screenshot Blocker
Stops the player's ability to screenshot or record your Unity game in **most** softwares. This plugin is useful if you have beta testers in your game and don't want them to record or capture any gameplay in your game.
Another thing that this can be useful is preventing any sensitive information (according to your game) to be leaked.
THIS DOES NOT WORK 100% OF THE TIME!
You can enable this in runtime, as well disabling it and restoring normal functionality.
## Using the Code (How to Block using Script)
Simply, to start blocking screenshots, you need to add this one line of code:
```c#
ScreenCaptureBlocker.Capture.ProtectWindowContent();
```
You can include this anywhere on runtime, commonly at Start() or Awake().

For example, if there's a certain area that you don't want the user to see and capture anything, use this on your code.

And if they exit the area, you can unblock it using:
```c#
ScreenCaptureBlocker.Capture.UnprotectWindowContent();
```
Here's a example code usage:
```c#
using UnityEngine;
using ScreenCaptureBlocker;
    
public class ExampleScript : MonoBehaviour 
{
  private bool VarA = false;
  private void Start()
  {
    Capture.ProtectWindowContent(); // Blocks screenshotting at start of the game.
  }
  
  private void Update()
  {
    if (!VarA && Time.timeSinceLevelLoad >= 15f) // If 15 seconds past, unblock screenshotting.
    {
      VarA = true;
      Capture.UnprotectWindowContent();
    }
  }
}
```
## Supported Platforms
Almost all major platforms are supported with this plugin:
 - Windows (Editor & Build)
 - Mac (Build Only)
 - Android
 - iOS
 
## What does it block?
It blocks the device's built-in feature for screenshotting or recording. This varies on each platform which can perform differently. 

Some of the softwares that are certainly be blocked:

 - Windows' Snipping Tool
 - Mac's Screenshot Tool
 - OBS' Screen & Window Capture
 - Any software that uses Windows' Screen Capture API

You should know that this does not block 100% of the time. **Do not use this plugin and be certain that it blocks everything.** There are some situations that can bypass this plugin.

This includes:
- OBS' Game Capture
- Mirroring your screen on Mac.
- Capture Cards (specifically Elgato's Game Capture Card)
- As well, taking a picture from your phone or another device.
