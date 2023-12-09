namespace AvaloniaExample

open System
open Avalonia
open AvaloniaExample
open Avalonia.ReactiveUI
open Projektanker.Icons.Avalonia
open Projektanker.Icons.Avalonia.MaterialDesign
open Projektanker.Icons.Avalonia.FontAwesome

module Program =

    [<CompiledName "BuildAvaloniaApp">] 
    let buildAvaloniaApp () = 

        IconProvider.Current
            .Register<MaterialDesignIconProvider>()
            .Register<FontAwesomeIconProvider>()
            |> ignore

        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(areas = Array.empty)
            .UseReactiveUI()

    [<EntryPoint; STAThread>]
    let main argv =
        buildAvaloniaApp().StartWithClassicDesktopLifetime(argv)
