open System.Runtime.Versioning
open Avalonia
open Avalonia.Browser
open Avalonia.ReactiveUI
open AvaloniaXPlatExample

module Program =
    [<assembly: SupportedOSPlatform("browser")>]
    do
        printfn "AvaloniaXPlatExample.Program starting"
        ()

    printfn "AvaloniaXPlatExample.Program build AvaloniaApp"
    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp () =
        AppBuilder
            .Configure<App>()

    [<EntryPoint>]
    let main argv =
        printfn "AvaloniaXPlatExample.main entry point"
        System.Diagnostics.Trace.Listeners.Add( new System.Diagnostics.ConsoleTraceListener()) |> ignore
        printfn "AvaloniaXPlatExample.main pre task"
        task {
            try
                printfn "AvaloniaXPlatExample.main build app"
                let app = buildAvaloniaApp()
                printfn "AvaloniaXPlatExample.main start app async"
                do! app.StartBrowserAppAsync("out")
                printfn "AvaloniaXPlatExample.main use reactive"
                app.UseReactiveUI() |> ignore
                printfn "AvaloniaXPlatExample.main set up logging"
                app.LogToTrace(
                    level = Logging.LogEventLevel.Debug
                        //areas = [|
                        //    //Logging.LogArea.Binding
                        //    //Logging.LogArea.Win32Platform
                        //    //Logging.LogArea.Control
                        //    //Logging.LogArea.Property
                        //    //Logging.LogArea.Visual
                        //    //Logging.LogArea.Animations
                        //    //Logging.LogArea.Platform
                        //|]
                )
                |> ignore
                printfn "AvaloniaXPlatExample.main exit"
            with ex ->
                printfn $"Exception: {ex.Message}\n{ex.StackTrace}"
            printfn "AvaloniaXPlatExample.main dropping out of task"
            return 0
        } |> ignore
        0
