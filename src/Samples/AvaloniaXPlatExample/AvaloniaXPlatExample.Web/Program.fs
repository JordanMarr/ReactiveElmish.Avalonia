open System.Runtime.Versioning
open Avalonia
open Avalonia.Browser
open Avalonia.ReactiveUI
open AvaloniaXPlatExample

open System.Runtime.Versioning
open System.Threading.Tasks
open Avalonia
open Avalonia.Browser
open Avalonia.ReactiveUI

module Program =
    [<assembly: SupportedOSPlatform("browser")>]
    do
        ()

    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp () = AppBuilder.Configure<App>()            

    [<EntryPoint>]
    let main argv =
        // System.Diagnostics.Trace.Listeners.Add( new System.Diagnostics.ConsoleTraceListener()) |> ignore
        task { 
            try
                do! buildAvaloniaApp()
                        .UseReactiveUI()
                        .StartBrowserAppAsync("out")
                    
                //let app = buildAvaloniaApp()
                //do! app.StartBrowserAppAsync("out")
                //app.UseReactiveUI() |> ignore
                //app.LogToTrace(
                //    level = Logging.LogEventLevel.Debug
                //        //areas = [|
                //        //    //Logging.LogArea.Binding
                //        //    //Logging.LogArea.Win32Platform
                //        //    //Logging.LogArea.Control
                //        //    //Logging.LogArea.Property
                //        //    //Logging.LogArea.Visual
                //        //    //Logging.LogArea.Animations
                //        //    //Logging.LogArea.Platform
                //        //|]
                //)
                //|> ignore
                return 0
            with ex ->
                printfn $"Exception: {ex.Message}\n{ex.StackTrace}"
                return 1
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
