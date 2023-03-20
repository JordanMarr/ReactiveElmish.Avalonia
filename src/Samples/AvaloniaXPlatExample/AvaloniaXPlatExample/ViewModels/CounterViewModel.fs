module AvaloniaXPlatExample.ViewModels.CounterViewModel

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
    {
        Count = 100
        Actions = [ { Description = "Initialized count."; Timestamp = DateTime.Now } ]
    }

let update (msg: Msg) (model: Model) =
    match msg with
    | Increment ->
        { model with
            Count = model.Count + 1
            Actions = model.Actions @ [ { Description = "Incremented"; Timestamp = DateTime.Now } ]
        }
    | Decrement ->
        { model with
            Count = model.Count - 1
            Actions = model.Actions @ [ { Description = "Decremented"; Timestamp = DateTime.Now } ]
        }
    | Reset ->
        init()

let bindings ()  : Binding<Model, Msg> list =
    [
        "Count" |> Binding.oneWay (fun m ->  m.Count)
        "Actions" |> Binding.oneWay (fun m ->  m.Actions)
        "Increment" |> Binding.cmd Increment
        "Decrement" |> Binding.cmd Decrement
        "Reset" |> Binding.cmd Reset
    ]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings
            |> AvaloniaProgram.withElmishErrorHandler
                (fun msg exn ->
                    printfn $"ElmishErrorHandlerCV: msg={msg}\n{exn.Message}\n{exn.StackTrace}"
                )
        )
