namespace AvaloniaExample.ViewModels

open System
open Elmish.Avalonia
open Elmish

module Counter = 
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
            { 
                Count = model.Count + 1 
                Actions = model.Actions @ [ { Description = "Incremented"; Timestamp = DateTime.Now } ]
            }
        | Decrement ->
            { 
                Count = model.Count - 1 
                Actions = model.Actions @ [ { Description = "Decremented"; Timestamp = DateTime.Now } ] 
            }
        | Reset ->
            init()

open Counter

type CounterViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init())

    member this.Count = this.BindModel(nameof this.Count, _.Count)
    member this.Actions = this.BindModel(nameof this.Actions, _.Actions)
    member this.Increment() = this.Dispatch Increment
    member this.Decrement() = this.Dispatch Decrement
    member this.Reset() = this.Dispatch Reset
    member this.IsResetEnabled = this.BindModel(nameof this.IsResetEnabled, fun m -> m.Count <> 0)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

    static member DesignVM = new CounterViewModel()