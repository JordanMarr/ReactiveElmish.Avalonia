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
        task { 
            try
                let app = buildAvaloniaApp()
                do! app.StartBrowserAppAsync("out")
                app.UseElmishBindings()
                |> ignore
                    (*
                    app.LogToTrace(Logging.LogEventLevel.Debug
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
                return 0
            with ex ->
                printfn $"Exception: {ex.Message}\n{ex.StackTrace}"
                return 1
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
