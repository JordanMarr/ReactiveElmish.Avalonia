module App

open System
open Elmish
open Elmish.Avalonia

type Model =  
    { 
        View: View
    }

and View = 
    | CounterView
    | ChartView
    | AboutView
    | FilePickerView

type Msg = 
    | SetView of View

let init () = 
    { 
        View = CounterView
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | SetView view -> { View = view }   

