module AvaloniaExample.ViewModels.AboutViewModel

open System
open Elmish.Avalonia

type Model = 
    {
        Version: string
    }

type Msg = | Msg

let init() = 
    { 
        Version = "1.0"
    }

let update (msg: Msg) (model: Model) = 
    model

let bindings ()  : Binding<Model, Msg> list = [
    "Version" |> Binding.oneWay (fun m -> m.Version)
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = Start(AvaloniaProgram.mkSimple init update bindings)