module AvaloniaExample.ViewModels.AboutViewModel

open System
open Elmish.Avalonia
open Elmish
open Messaging

type Model = 
    {
        Version: string
    }

type Msg = 
    | Ok
    | MsgSent of unit

let init() = 
    { 
        Version = "1.0"
    }, Cmd.none

let update (msg: Msg) (model: Model) = 
    match msg with
    | Ok -> 
        let sendMessage () = bus.OnNext(GlobalMsg.GoHome)
        model, Cmd.OfFunc.perform sendMessage () MsgSent

    | MsgSent _ -> 
        model, Cmd.none

let bindings ()  : Binding<Model, Msg> list = [
    "Version" |> Binding.oneWay (fun m -> m.Version)
    "Ok" |> Binding.cmd Ok
]

let designVM = ViewModel.designInstance (fst (init())) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkProgram init update bindings)
