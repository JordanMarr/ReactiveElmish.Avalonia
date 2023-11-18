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

type CounterViewModel(app: IElmishStore<App.Model, App.Msg>) =
    inherit ReactiveElmishViewModel()

    let counter = 
        Program.mkAvaloniaSimple init update
        //|> Program.withConsoleTrace
        |> Program.mkStore

    member this.Count = this.Bind(counter, _.Count)
    member this.Actions = this.Bind(counter, _.Actions)
    member this.Increment() = counter.Dispatch Increment
    member this.Decrement() = counter.Dispatch Decrement
    member this.Reset() = counter.Dispatch Reset
    member this.IsResetEnabled = this.Bind(counter, fun m -> m.Count <> 0)

    static member DesignVM = 
        let store = Store.design(App.init())
        new CounterViewModel(store)