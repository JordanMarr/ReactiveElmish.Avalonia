namespace AvaloniaXPlatExample.ViewModels

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
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        //|> Program.withConsoleTrace
        |> Program.mkStore

    member this.Count = this.Bind(local, _.Count)
    member this.Actions = this.Bind(local, _.Actions)
    member this.Increment() = local.Dispatch Increment
    member this.Decrement() = local.Dispatch Decrement
    member this.Reset() = local.Dispatch Reset
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Count <> 0)

    static member DesignVM = 
        new CounterViewModel() 