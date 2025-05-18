using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Diagnostics;

namespace ScreenCaptureBlocker
{
    public class RecChecker : MonoBehaviour
    {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        // This code does increase CPU usage by 15% especially if it's called in an interval.
        private static bool IsRunningARecording()
        {
            Process[] processlist = Process.GetProcessesByName("screencapture");
            return processlist.Length > 0;
        }
#elif UNITY_STANDALONE_WIN
        // *******************************************************************************
        // Not used unless you uncomment the stuff here and on SCB/Capture.cs. For OBS Game Capture Detection.
        // [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        // private static extern System.IntPtr GetModuleHandle(string lpModuleName);

        private static bool IsRunningARecording()
        {
            // if (GetModuleHandle("graphics-hook" + ((System.IntPtr.Size == 8) ? "64" : "32") + ".dll") != System.IntPtr.Zero) // If a plugin loads, it never unloads.
            // {
            //     // Crashes game.
            //     UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.Abort);
            //     return true;
            // }
            return false;
        }
        // *******************************************************************************
#else
        private static bool IsRunningARecording() { return false; }
#endif

        //************************************************************************************
        // The reason why it has a delayed interval is to prevent excessive CPU consumption being made
        // You may lower it down, but you may experience more CPU usage and slowness within your game.
        //
        // Or, if you care less about strictness, you can go to SCB/Capture.cs and set BLACKOUT_IF_QUICKTIME_RECORDING to false
        private const float INTERVAL = 1f;
        //************************************************************************************

        private float time = INTERVAL;

        public static GameObject Instance;

        private bool HasOpened;

        private GameObject gb;

        // Fixed update is preferred for better performance.
        private void FixedUpdate()
        {
            time -= Time.fixedUnscaledDeltaTime;
            if (time <= 0f)
            {
                time = INTERVAL;
                CheckIfOpen();
            }
        }

        private void OnDestroy()
        {
            if (gb != null)
            {
                Destroy(gb);
            }
        }

        private void CheckIfOpen()
        {
            bool NowOpen = IsRunningARecording();
            if (HasOpened != NowOpen)
            {
                if (NowOpen)
                {
                    // Show a black screen in the game.
                    gb = new GameObject("ALERT__");
                    DontDestroyOnLoad(gb);
                    Canvas a1 = gb.AddComponent<Canvas>();
                    a1.renderMode = RenderMode.ScreenSpaceOverlay;
                    a1.sortingOrder = 9999;
                    CanvasScaler a2 = gb.AddComponent<CanvasScaler>();
                    a2.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    a2.referenceResolution = new Vector2(800, 800);
                    GraphicRaycaster raycaster = gb.AddComponent<GraphicRaycaster>();

                    GameObject BlackScreenGb = new GameObject("BG");
                    BlackScreenGb.transform.SetParent(gb.transform);

                    RawImage ri = BlackScreenGb.AddComponent<RawImage>();
                    ri.raycastTarget = false;
                    ri.color = Color.black;
                    ri.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                    ri.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                    ri.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
                else
                {
                    OnDestroy();
                }
            }
            HasOpened = NowOpen;
        }
    }
}