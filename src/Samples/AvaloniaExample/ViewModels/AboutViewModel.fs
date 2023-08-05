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


let bindings ()  : Binding<Model, Msg> list = [
    "Version" |> Binding.oneWay (fun m -> m.Version)
    "Ok" |> Binding.cmd Ok
]

let designVM = ViewModel.designInstance (fst (init())) (bindings())

let vm = 
    AvaloniaProgram.mkProgram init update bindings
    |> ElmishViewModel.create
    |> ElmishViewModel.terminateOnViewUnloaded Terminate
    |> ElmishViewModel.subscribe (fun view model dispatch -> 
        view.Loaded |> Observable.subscribe (fun _ -> 
            printfn "View Loaded!"
        )
    )
