namespace AvaloniaXPlatExample.Desktop
open System
open Avalonia
open AvaloniaXPlatExample
open Avalonia.ReactiveUI

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
            .UseReactiveUI()


    [<EntryPoint; STAThread>]
    let main argv =
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(argv) |> ignore
        0
