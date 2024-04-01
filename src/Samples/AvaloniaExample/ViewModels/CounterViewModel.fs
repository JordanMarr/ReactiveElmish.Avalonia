namespace AvaloniaExample.ViewModels

open System
open ReactiveElmish
open ReactiveElmish.Avalonia
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
            Actions = [ { Description = "Initialized Counter"; Timestamp = DateTime.Now } ]
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
            {
                Count = 0 
                Actions = [ { Description = "Reset"; Timestamp = DateTime.Now } ]
            }

open Counter

type CounterViewModel() =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    member this.Count = this.Bind(local, _.Count)
    member this.Actions = 
        this.BindList(
        local
        , _.Actions 
        , map = fun a -> { a with Description = $"** {a.Description} **" }
        , sortBy = _.Timestamp
    )
    member this.Increment() = local.Dispatch Increment
    member this.Decrement() = local.Dispatch Decrement
    member this.Reset() = local.Dispatch Reset
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Actions.Length > 1)

    static member DesignVM = 
        new CounterViewModel()