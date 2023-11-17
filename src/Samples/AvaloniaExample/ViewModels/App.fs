module App

open System
open Elmish
open Elmish.Avalonia

type Model =  
    { 
        View: View
        Count: int
        Actions: Action list 
        Version: string
    }
and Action = { Description: string; Timestamp: DateTime }
and View = 
    | CounterView
    | ChartView
    | AboutView
    | FilePickerView

type Msg = 
    | SetView of View
    | Increment
    | Decrement
    | Reset

let init () = 
    { 
        View = CounterView
        Count = 0
        Actions = [ { Description = "Initialized count."; Timestamp = DateTime.Now } ]
        Version = "1.1"
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
        { model with 
            Count = 0
            Actions = [ { Description = "Reset"; Timestamp = DateTime.Now } ]
        }
    | SetView view -> 
        { model with 
            View = view
            Actions = model.Actions @ [ { Description = "Set View"; Timestamp = DateTime.Now } ] 
        }   

