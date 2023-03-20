namespace AvaloniaXPlatExample.Desktop
open System
open Avalonia
open AvaloniaXPlatExample
open Elmish.Avalonia.AppBuilder

open System.Diagnostics
module Program =

    System.Diagnostics.Trace.Listeners.Add( new ConsoleTraceListener()) |> ignore
    let BuildAvaloniaApp() =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(Logging.LogEventLevel.Verbose,
                            areas=[|
                                Logging.LogArea.Binding
                                Logging.LogArea.Win32Platform
                                // Logging.LogArea.Control
                                //Logging.LogArea.Property
                                //Logging.LogArea.Visual
                            |]
                        )
            .UseElmishBindings()


    [<EntryPoint; STAThread>]
    let main argv =
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(argv) |> ignore
        printfn $"YYY: before main thread exit"
        0
