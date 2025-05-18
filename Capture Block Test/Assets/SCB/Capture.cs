using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ScreenCaptureBlocker
{
    public static class Capture
    {
        /******************************************************************************
            *                      SETTINGS YOU CAN MODIFY                      *
        ******************************************************************************/
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // (WINDOWS ONLY) Hides the Game Window in the capture's side instead of blacking out the content.
        private static readonly bool HIDE_WINDOW = false;
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // (MAC ONLY) If a Quicktime Recording is happening, black out the whole application until it stops.
        // NOTE: This can cause an increase in CPU usage as applications are being detected in an interval.
        // See SCB/RecChecker.cs
        private static readonly bool BLACKOUT_IF_QUICKTIME_RECORDING = true;
#endif
#if UNITY_WEBGL
        // Enable Web Support
        // This only blocks common screenshot shortcuts and not Screen Captures.
        private static readonly bool ENABLE_WEBGL_PARTIALSUPPORT = false; // If disabled, it'll throw an error and redirect to this.
        // (WEBGL ONLY) Blurs background instead of being a background color. NOTE: Without blurring, printing is fully blocked.
        private static readonly bool BLUR = false;
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private const uint WDA_NONE = 0x00000000;
        private const uint WDA_MONITOR = 0x00000001;
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
        private const uint SUCCESS = 1U;

        public static IntPtr GetWindow()
        {
            IntPtr ActiveWindow = GetActiveWindow();
            if (ActiveWindow.ToInt32() == 0)
            {
                return Process.GetCurrentProcess().MainWindowHandle;
            }
            return ActiveWindow;
        }
#endif

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
        [DllImport("ScreenCaptureProtector")]
        private static extern void SetNoSharingToWindow();

        [DllImport("ScreenCaptureProtector")]
        private static extern void SetReadSharingToWindow();

        [DllImport("ScreenCaptureProtector")]
        private static extern bool IsWindowSharingAllowed();
#endif

#if UNITY_EDITOR_OSX
        private static void SetNoSharingToWindow() {}

        private static void SetReadSharingToWindow() {}

        private static bool IsWindowSharingAllowed() { return true; }
#endif

#if UNITY_ANDROID
        private const int FLAG_SECURE = 0x00002000;

        private static void addFlags(int value)
        {
            int flagsValue = value;
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow", Array.Empty<object>()))
                    {
                        window.Call("addFlags", new object[]
                        {
                            flagsValue
                        });
                    }
                }
            }
        }

        private static void clearFlags(int value)
        {
            int flagsValue = value;
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow", Array.Empty<object>()))
                    {
                        window.Call("clearFlags", new object[]
                        {
                            flagsValue
                        });
                    }
                }
            }
        }
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void addSecureView();

        [DllImport("__Internal")]
        private static extern void removeSecureView();
#endif

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ApplyProtectionWithAppName(string str, bool blur);

        [DllImport("__Internal")]
        private static extern void DeapplyProtection();
#endif

        /// <summary>
        /// Blocks the application from being captured by most screen capturers/recorders.
        /// </summary>
        public static void ProtectWindowContent()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            /***** WINDOWS *****/
            if (SetWindowDisplayAffinity(GetWindow(), HIDE_WINDOW ? WDA_EXCLUDEFROMCAPTURE : WDA_MONITOR) != SUCCESS)
            {
                throw new MissingReferenceException("[Content Capture Protection] Failed to retrieve window.");
            }
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");

            // Discouraged. If you still like to use, uncomment it out.
            // if (CRASHGAME_IF_OBSGAMECAPTURE_PRESENT)
            // {
            //     GameObject gmo = new GameObject("Crash Detector for SCB");
            //     gmo.AddComponent<ScreenCaptureBlocker.RecChecker>();
            //     UnityEngine.Object.DontDestroyOnLoad(gmo);
            //     ScreenCaptureBlocker.RecChecker.Instance = gmo;
            // }
#if UNITY_EDITOR
            GameObject gmo2 = new GameObject("Capture Blocked (DO NOT DELETE) This Will Unblock On Exit");
            gmo2.AddComponent<ScreenCaptureBlocker.OnQuit>();
            UnityEngine.Object.DontDestroyOnLoad(gmo2);
            ScreenCaptureBlocker.OnQuit.Instance = gmo2;
#endif
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            /***** MAC *****/
            SetNoSharingToWindow();
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");

            if (BLACKOUT_IF_QUICKTIME_RECORDING)
            {
                GameObject gmo3 = new GameObject("Rec Detector for SCB");
                gmo3.AddComponent<ScreenCaptureBlocker.RecChecker>();
                UnityEngine.Object.DontDestroyOnLoad(gmo3);
                ScreenCaptureBlocker.RecChecker.Instance = gmo3;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("[Content Capture Protection] Editor is not affected by Capture Protection, but it will be at build.");
#endif
#elif UNITY_WSA || UNITY_WSA_10_0
            /***** UWP *****/
#if ENABLE_WINMD_SUPPORT // Supported when Windows Runtime API is implemented
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = false;
#else
            throw new NotSupportedException("Windows.UI.ViewManagement / WinMD is unavailable");
#endif
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");
#elif UNITY_ANDROID
            /***** ANDROID *****/
            addFlags(FLAG_SECURE);
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");
#elif UNITY_IOS
            /***** IOS *****/
            addSecureView();
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");
#elif UNITY_WEBGL
            if (ENABLE_WEBGL_PARTIALSUPPORT)
            {
                ApplyProtectionWithAppName(Application.productName, BLUR);
                UnityEngine.Debug.Log("[Content Capture Protection] Protection status is active.");
                return;
            }
            throw new NotSupportedException("[Content Capture Protection] Support not enabled. WebGL is partially supported, but should be advised. Proceed by editing SCB/Capture.cs and setting ENABLE_WEBGL_PARTIALSUPPORT to true. This only blocks screenshot shortcuts.");
#else
            UnityEngine.Debug.LogWarning("[Content Capture Protection] This platform is not supported.");
#endif
        }

        /// <summary>
        /// Unblocks the application from being captured by most screen capturers/recorders.
        /// </summary>
        public static void UnprotectWindowContent()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            /***** WINDOWS *****/
            if (SetWindowDisplayAffinity(GetWindow(), WDA_NONE) != SUCCESS)
            {
                throw new UnauthorizedAccessException("[Content Capture Protection] Failed to retrieve window.");
            }
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
            if (ScreenCaptureBlocker.RecChecker.Instance != null)
            {
                UnityEngine.Object.Destroy(ScreenCaptureBlocker.RecChecker.Instance);
                ScreenCaptureBlocker.RecChecker.Instance = null;
            }
#if UNITY_EDITOR
            if (ScreenCaptureBlocker.OnQuit.Instance != null)
            {
                UnityEngine.Object.Destroy(ScreenCaptureBlocker.OnQuit.Instance);
                ScreenCaptureBlocker.OnQuit.Instance = null;
            }
#endif
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            /***** MAC *****/
            SetReadSharingToWindow();
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
            if (ScreenCaptureBlocker.RecChecker.Instance != null)
            {
                UnityEngine.Object.Destroy(ScreenCaptureBlocker.RecChecker.Instance);
                ScreenCaptureBlocker.RecChecker.Instance = null;
            }
#elif UNITY_WSA || UNITY_WSA_10_0
            /***** UWP *****/
#if ENABLE_WINMD_SUPPORT // Supported when Windows Runtime API is implemented
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = true;
#endif
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
#elif UNITY_ANDROID
            /***** ANDROID *****/
            clearFlags(FLAG_SECURE);
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
#elif UNITY_IOS
            /***** IOS *****/
            removeSecureView();
            UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
#elif UNITY_WEBGL
            if (ENABLE_WEBGL_PARTIALSUPPORT)
            {
                DeapplyProtection();
                UnityEngine.Debug.Log("[Content Capture Protection] Protection status is deactivated.");
            }
#endif
        }

        // UNUSED SETTINGS
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        /***
        // (WINDOWS ONLY) [Build] If OBS is capturing the game itself, crash the whole application. NOTE: Once detected, it's always detected until you restart the whole application.
        // Very discouraged. If you still want to, uncomment this out (remove the /*** & ***\/) and in the ProtectWindowContent()
        private static readonly bool CRASHGAME_IF_OBSGAMECAPTURE_PRESENT = true;
        ***/
#endif
    }
}
/******************************************************************************
Copyright © 2025 SeenWonderAlex

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the “Software”), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
******************************************************************************/