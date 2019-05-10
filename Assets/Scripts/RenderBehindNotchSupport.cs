using UnityEngine;

public class RenderBehindNotchSupport : MonoBehaviour
{
    public bool RenderBehindNotch = true;
    
    // Constants from https://developer.android.com/reference/android/view/WindowManager.LayoutParams.html
    private const int LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES = 1;
    private const int LAYOUT_IN_DISPLAY_CUTOUT_MODE_NEVER = 2;

    private AndroidJavaObject m_Window;
    private AndroidJavaObject m_Windowattributes;

    public void SetRenderBehindNotch(bool enabled)
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            // Supported on Android 9 Pie (API 28) and later
            if (version.GetStatic<int>("SDK_INT") < 28)
            {
                return;
            }
        }
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                m_Window = activity.Call<AndroidJavaObject>("getWindow");
                m_Windowattributes = m_Window.Call<AndroidJavaObject>("getAttributes");
                m_Windowattributes.Set("layoutInDisplayCutoutMode", enabled ?
                    LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES :
                    LAYOUT_IN_DISPLAY_CUTOUT_MODE_NEVER);
                activity.Call("runOnUiThread", new AndroidJavaRunnable(ApplyAttributes));
            }
        }
    }

    void Start ()
    {
        SetRenderBehindNotch(RenderBehindNotch);
    }

    void ApplyAttributes()
    {
        if (m_Window != null && m_Windowattributes != null)
            m_Window.Call("setAttributes", m_Windowattributes);
    }
}
