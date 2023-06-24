module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open CommunityToolkit.Mvvm.Input
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.Defaults
open LiveChartsCore.SkiaSharpView

let _random = Random()

type Model = 
    {
        Data: int list
        Actions: Action list
    }
    
and Action = 
    {
        Description: string
    }

type Msg = 
    | AddItem

let init() = 
    { 
        Data = []
        Actions = [ { Description = "AddItem"} ]
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | AddItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]            
            Data = model.Data @ [ _random.Next(0, 10) ]
        }

let bindings ()  : Binding<Model, Msg> list = [
    "AddItem" |> Binding.cmd AddItem
    "Series" |> Binding.oneWay (fun m -> 
        [|
            LineSeries<int>(Values = m.Data, Fill = null, Name = "Income") :> ISeries
        |]
    )
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)