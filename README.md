# UnityAndroidNotchSupport
Android devices with a notch (display cutout) have recently become quite common. However, if you make a Unity application and deploy it to such device, by default Android OS will letter-box your app so that it the notch doesn't interfere with your UI.

If you want to make use of the whole display surface area and render in the area "behind" the notch, you need to add code to your application. Fortunately, since Unity 2018.3 there is a special option in the Player settings called "Render outside safe area" which does exactly this. If you want to have the same option in earlier versions of Unity - this plugin was made for you!

One advantage of this plugin over the built-in Unity solution is that it allows changing the setting in runtime if needed, by calling `public void SetRenderBehindNotch(bool enabled)` in `RenderBehindNotchSupport`.

This plugin is targeted towards Unity 2017.4, however I see no reasons why it shouldn't work with earlier versions too.

## System Requirements
-	Tested on Unity 2017.4.28f1. Should work on any Unity version out there, but make sure your target API is set to 28 or higher. There is no point in using this plugin in Unity 2018.3 or later because these versions have notch support out of the box.
-	An Android device with Android 9 Pie or later. Some devices with earlier Android versions have notch/cutout, but Google has added a corresponding API only in Android 9. Feel free to add other vendor-specific bits of code to add support on earlier Androids at your own risk.

## Usage
1.	Copy the contents of `Assets` directory to your project
2.	Attach the `Assets/Scripts/RenderBehindNotchSupport.cs` script to a game object of your choice in your first scene to make sure the plugin is loaded as early as possible
3.	The script has a public boolean property so that you can tick/untick the checkbox to enable or disable rendering behind the notch with a single click
4.	If you want to change the setting in runtime, call `public void SetRenderBehindNotch(bool enabled)` in `RenderBehindNotchSupport` class.
5.	Enjoy

## Useful Links
-	https://developer.android.com/guide/topics/display-cutout
-	https://developer.android.com/reference/android/view/WindowManager.LayoutParams#layoutInDisplayCutoutMode

## License
Licensed under MIT license.
