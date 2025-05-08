using UnityEngine;

namespace ScreenCaptureBlocker
{
    public class OnQuit : MonoBehaviour
    {
#if UNITY_EDITOR
        public static GameObject Instance;

        private void OnApplicationQuit()
        {
            Capture.UnprotectWindowContent(); // This is to prevent the editor being blocked when it's no longer in playing mode.
        }
#endif
    }
}