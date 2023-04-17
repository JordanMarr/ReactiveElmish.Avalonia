module AvaloniaExample.ViewModels.AboutViewModel

open System
open Elmish.Avalonia
open Elmish
open Messaging

type Model = 
    {
        Version: string
        CurrentCount: int
    }

type Msg = 
    | Ok
    | MsgSent of unit

let init currentCount = 
    { 
        Version = "1.0"
        CurrentCount = currentCount
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
    "CurrentCount" |> Binding.oneWay (fun m -> m.CurrentCount)
]

let designVM = ViewModel.designInstance (fst (init 0)) (bindings())

let vm currentCount = ElmishViewModel(AvaloniaProgram.mkProgram (fun () -> init currentCount) update bindings)
