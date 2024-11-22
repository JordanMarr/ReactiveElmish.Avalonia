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

type ActionVM(action: Action) =
    inherit ReactiveElmishViewModel()
    
    member this.Timestamp = action.Timestamp
    member this.Description = $"** {action.Description} **"

    interface IDisposable with
        member this.Dispose() =
            printfn "Disposing ActionVM"


type CounterViewModel() as this =
    inherit ReactiveElmishViewModel()

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.mkStore

    let mkAction action = 
        let vm = new ActionVM(action)
        this.AddDisposable(vm) // Dispose child vms when parent vm is disposed
        vm

    member this.Count = this.Bind(local, _.Count)
    member this.Actions = 
        this.BindList(
        local
        , _.Actions 
        , map = mkAction
        , sortBy = _.Timestamp
    )
    member this.Increment() = local.Dispatch Increment
    member this.Decrement() = local.Dispatch Decrement
    member this.Reset() = local.Dispatch Reset
    member this.IsResetEnabled = this.Bind(local, fun m -> m.Actions.Length > 1)

    static member DesignVM = 
        new CounterViewModel()