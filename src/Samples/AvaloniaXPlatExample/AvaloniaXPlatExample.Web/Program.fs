open System.Runtime.Versioning
open Avalonia
open Avalonia.Browser

open AvaloniaXPlatExample
open Elmish.Avalonia.AppBuilder

module Program =
    [<assembly: SupportedOSPlatform("browser")>]
    do
        ()

    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp () =
        AppBuilder
            .Configure<App>()

    [<EntryPoint>]
    let main argv =
        // System.Diagnostics.Trace.Listeners.Add( new System.Diagnostics.ConsoleTraceListener()) |> ignore
        try
            buildAvaloniaApp()
                (*
                .LogToTrace(Logging.LogEventLevel.Debug
                            areas=[|
                                //Logging.LogArea.Binding
                                //Logging.LogArea.Win32Platform
                                //Logging.LogArea.Control
                                //Logging.LogArea.Property
                                //Logging.LogArea.Visual
                                //Logging.LogArea.Animations
                                //Logging.LogArea.Platform
                            |]
                        )
                *)
                .UseElmishBindings()
                .SetupBrowserApp("out")
                |> ignore
        with x ->
            printfn $"Exception: {x.Message}\n{x.StackTrace}"
        0
