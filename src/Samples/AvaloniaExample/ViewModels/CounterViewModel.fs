namespace AvaloniaExample.ViewModels

open System
open Elmish.Avalonia
open Elmish
open DynamicData

module Counter = 
    type Model =  { Count: int; Actions: SourceList<Action> }
    and Action = { Description: string; Timestamp: DateTime }

    type Msg = 
        | Increment
        | Decrement
        | Reset

    let init() = 
        let actions = new SourceList<Action>()
        actions.Add { Description = "Initialized Counter"; Timestamp = DateTime.Now }
        { Count = 0; Actions = actions }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | Increment ->
            model.Actions.Add { Description = "Incremented"; Timestamp = DateTime.Now }
            { model with Count = model.Count + 1 }
        | Decrement ->
            model.Actions.Add { Description = "Decremented"; Timestamp = DateTime.Now }
            { model with Count = model.Count - 1 }
        | Reset ->
            model.Actions.Clear()
            model.Actions.Add { Description = "Reset"; Timestamp = DateTime.Now }
            { model with Count = 0 }


open Counter

type CounterViewModel() as this =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    let mutable actions = Unchecked.defaultof<_>
    do local.Model.Actions.Connect().Bind(&actions).Subscribe() |> this.AddDisposable

    member this.Count = this.Bind(local, _.Count)
    member this.Actions = actions
    member this.Increment() = local.Dispatch Increment
    member this.Decrement() = local.Dispatch Decrement
    member this.Reset() = local.Dispatch Reset
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Count <> 0)

    static member DesignVM = 
        new CounterViewModel()