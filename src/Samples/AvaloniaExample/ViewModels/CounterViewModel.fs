module AvaloniaExample.ViewModels.CounterViewModel

open System
open Elmish.Avalonia

type Model = 
    {
        Count: int
        Actions: Action list
    }

and Action = 
    {
        Description: string
        Timestamp: DateTime
    }

type Msg = 
    | Increment
    | Decrement
    | Reset

let init() = 
    let count = 100
    Messaging.bus.OnNext(Messaging.GlobalMsg.SetCount count)
    { 
        Count = count
        Actions = [ { Description = "Initialized count."; Timestamp = DateTime.Now } ]
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | Increment ->
        let count = model.Count + 1
        Messaging.bus.OnNext(Messaging.GlobalMsg.SetCount count)
        { model with 
            Count = count
            Actions = model.Actions @ [ { Description = "Incremented"; Timestamp = DateTime.Now } ]
        }
    | Decrement ->
        let count = model.Count - 1
        Messaging.bus.OnNext(Messaging.GlobalMsg.SetCount count)
        { model with 
            Count = count
            Actions = model.Actions @ [ { Description = "Decremented"; Timestamp = DateTime.Now } ]
        }
    | Reset ->
        init()

let bindings ()  : Binding<Model, Msg> list = [
    "Count" |> Binding.oneWay (fun m -> m.Count)
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "Increment" |> Binding.cmd Increment
    "Decrement" |> Binding.cmd Decrement
    "Reset" |> Binding.cmd Reset
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)