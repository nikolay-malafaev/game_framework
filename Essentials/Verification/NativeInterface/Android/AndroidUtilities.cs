using UnityEngine;

namespace GameFramework.Verification.NativeInterface
{
    internal static class AndroidUtilities
    {
        private static AndroidJavaObject unityActivity;

        public static AndroidJavaObject GetUnityActivity()
        {
            if (unityActivity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return unityActivity;
        }
    }
}