module AvaloniaExample.ViewModels.CounterViewModel

open System
open Elmish.Avalonia
open Elmish

type Model =  { Count: int; Actions: Action list }
and Action = { Description: string; Timestamp: DateTime }

type Msg = 
    | Increment
    | Decrement
    | Reset

let init() = 
    { 
        Count = 0
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

type CounterViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init())

    member this.Count = this.BindModel(fun m -> m.Count)
    member this.Actions = this.BindModel(fun m -> m.Actions)
    member this.Increment() = this.Dispatch Msg.Increment
    member this.Decrement() = this.Dispatch Msg.Decrement
    member this.Reset() = this.Dispatch Msg.Reset

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

let designVM = new CounterViewModel()