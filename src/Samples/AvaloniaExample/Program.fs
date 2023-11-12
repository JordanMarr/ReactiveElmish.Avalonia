namespace AvaloniaExample

open System
open Avalonia
open AvaloniaExample
open Elmish.Avalonia.AppBuilder
open Avalonia.ReactiveUI

module Program =

    [<CompiledName "BuildAvaloniaApp">] 
    let buildAvaloniaApp () = 
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(areas = Array.empty)
            .UseElmishBindings()
            .UseReactiveUI()

    [<EntryPoint; STAThread>]
    let main argv =
        buildAvaloniaApp().StartWithClassicDesktopLifetime(argv)
