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

let init() = 
    { 
        Version = "1.1"
    }, Cmd.none

let update (msg: Msg) (model: Model) = 
    match msg with
    | Ok -> 
        model, Cmd.ofEffect (fun _ -> bus.OnNext(GlobalMsg.GoHome))

let bindings ()  : Binding<Model, Msg> list = [
    "Version" |> Binding.oneWay (fun m -> m.Version)
    "Ok" |> Binding.cmd Ok
]

let designVM = ViewModel.designInstance (fst (init())) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkProgram init update bindings)
