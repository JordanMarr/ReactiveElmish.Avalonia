module AvaloniaExample.ViewModels.MainViewModel

open System
open Elmish
open Elmish.Avalonia

type Model = 
    {
        ContentVM: IStart
    }

type Msg = 
    | Msg

let init() = 
    { 
        ContentVM = CounterViewModel.vm
    }

let update (msg: Msg) (model: Model) = 
    model

let bindings() : Binding<Model, Msg> list = [
    // Properties
    "ContentVM" |> Binding.oneWay (fun m -> m.ContentVM)
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm : IStart = Start(AvaloniaProgram.mkSimple init update bindings)