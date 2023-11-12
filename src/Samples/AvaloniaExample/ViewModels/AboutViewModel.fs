module AvaloniaExample.ViewModels.AboutViewModel

open Elmish.Avalonia
open Elmish
open Messaging

type Model = 
    {
        Version: string
    }

type Msg = 
    | Ok
    | Terminate

let init() = 
    { 
        Version = "1.1"
    }, Cmd.none

let update (msg: Msg) (model: Model) = 
    match msg with
    | Ok -> model, Cmd.ofEffect (fun _ -> bus.OnNext(GlobalMsg.GoHome))
    | Terminate -> model, Cmd.none

type AboutViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init() |> fst)

    member this.Version = this.BindModel(fun m -> m.Version)
    member this.Ok() = this.Dispatch Msg.Ok

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaProgram init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

let designVM = new AboutViewModel()