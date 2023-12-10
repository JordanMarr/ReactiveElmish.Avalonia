namespace AvaloniaXPlatExample.Android
open Android.App
open Android.Content
open Android.Content.PM
open Android.OS
open Avalonia.Android
open Avalonia.ReactiveUI
type Application = Android.App.Application

open Avalonia.Android
open Avalonia.Android
open AvaloniaXPlatExample

[<Activity(
    Label = "AvaloniaXPlatExample.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = (ConfigChanges.Orientation ||| ConfigChanges.ScreenSize))>]
type MainActivity() =
    inherit AvaloniaMainActivity<App>()

    override _.CustomizeAppBuilder(builder) =
        base.CustomizeAppBuilder(builder)
            .UseReactiveUI()

//[<Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)>]
//type SplashActivity() =
//    //inherit AvaloniaSplashActivity<App>()
//    inherit AvaloniaSplashActivity

//    override _.CustomizeAppBuilder(builder) =
//        base.CustomizeAppBuilder(builder);

//    override x.OnResume() =
//        base.OnResume()
//        x.StartActivity(new Intent(Application.Context, typeof<MainActivity>))
