using UnityEngine;

public class WindowInsetsListener : AndroidJavaProxy
{
    public struct Insets
    {
        public int left, top, right, bottom;
    }

    private static Insets m_safeInset = new Insets();
    private static Object m_safeInsetLock = new Object();

    public static Insets SafeInset
    {
        get
        {
            return m_safeInset;
        }
    }

    public void InitInsets(AndroidJavaObject decorView)
    {
        onApplyWindowInsets(decorView, decorView.Call<AndroidJavaObject>("getRootWindowInsets"));
    }

    public WindowInsetsListener() : base("android.view.View$OnApplyWindowInsetsListener") { }

    private AndroidJavaObject onApplyWindowInsets(AndroidJavaObject view, AndroidJavaObject windowInsets)
    {
        using (var cutout = windowInsets.Call<AndroidJavaObject>("getDisplayCutout"))
        {
            if (cutout == null)
            {
                lock (m_safeInsetLock)
                {
                    m_safeInset.left = 0;
                    m_safeInset.top = 0;
                    m_safeInset.right = 0;
                    m_safeInset.bottom = 0;
                }
            }
            else
            {
                lock (m_safeInsetLock)
                {
                    m_safeInset.left = cutout.Call<int>("getSafeInsetLeft");
                    m_safeInset.top = cutout.Call<int>("getSafeInsetTop");
                    m_safeInset.right = cutout.Call<int>("getSafeInsetRight");
                    m_safeInset.bottom = cutout.Call<int>("getSafeInsetBottom");
                }
            }
        }

        return view.Call<AndroidJavaObject>("onApplyWindowInsets", windowInsets);
    }
}

public class AndroidSafeArea : MonoBehaviour
{
    private static bool m_insetsListenerInstalled = false;

	void Start()
	{
	    if (m_insetsListenerInstalled)
	        return;

	    using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
	    {
	        // Supported on Android 9 Pie (API 28) and later
	        if (version.GetStatic<int>("SDK_INT") >= 28)
	        {
                // Install the listener
	            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
	            {
	                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
	                {
	                    using (var window = activity.Call<AndroidJavaObject>("getWindow"))
	                    {
	                        using (var decorView = window.Call<AndroidJavaObject>("getDecorView"))
	                        {
                                var windowInsetsListener = new WindowInsetsListener();
	                            windowInsetsListener.InitInsets(decorView);
                                decorView.Call("setOnApplyWindowInsetsListener", windowInsetsListener);
	                            m_insetsListenerInstalled = true;
	                        }
                        }
                    }
	            }
            }
	    }
    }

    public static Rect safeArea
    {
        get
        {
            if (!m_insetsListenerInstalled)
                return Screen.safeArea;

            // Scale in case resolution doesn't match the native one
            float xScale = (float)Screen.width / Display.main.systemWidth;
            float yScale = (float)Screen.height / Display.main.systemHeight;

            Rect safeArea = new Rect
            {
                x       = Mathf.Round(WindowInsetsListener.SafeInset.left * xScale),
                width   = Mathf.Round(Screen.width - (WindowInsetsListener.SafeInset.left + WindowInsetsListener.SafeInset.right) * xScale),
                y       = Mathf.Round(WindowInsetsListener.SafeInset.top * yScale),
                height  = Mathf.Round(Screen.height - (WindowInsetsListener.SafeInset.bottom + WindowInsetsListener.SafeInset.top) * yScale)
            };

            return safeArea;
        }
    }
}
