module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let _random = Random()
let observableValues = ObservableCollection<ObservableValue>()
let series = 
    ObservableCollection<ISeries> 
        [ 
            LineSeries<ObservableValue>(Values = observableValues) :> ISeries 
        ]
        
let createNewSeries =
    for _ in 1 .. 10 do
        let randomValue = _random.Next(1, 11)
        let observableValue = ObservableValue(randomValue)
        observableValues.Add(observableValue)
        
let replaceItemAtPosition (position: int) (newValue: ObservableValue) =
    if position >= 0 && position < observableValues.Count then
        observableValues.[position] <- newValue

type Model = 
    {
        Actions: Action list
    }
    
and Action = 
    {
        Description: string
    }

type Msg = 
    | AddItem
    | RemoveItem
    | UpdateItem
    | ReplaceItem
    | Reset

let init() =
    createNewSeries
    { 
        Actions = [ { Description = "AddItem"} ]
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | AddItem ->
        observableValues.Add(ObservableValue(_random.Next(0, 11)))
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]    
        }
    | RemoveItem ->
        observableValues.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]    
        }
    | UpdateItem ->
        replaceItemAtPosition (_random.Next(0, observableValues.Count-1)) (ObservableValue(_random.Next(0, 11)))
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
        }
    | ReplaceItem ->
        observableValues.RemoveAt(observableValues.Count-1)
        observableValues.Add(ObservableValue(_random.Next(0, 10)))
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
        }
    | Reset ->
        // why can I not just call "createNewSeries" here?
        observableValues.Clear()
        for _ in 1 .. 10 do
            let randomValue = _random.Next(1, 11)
            let observableValue = ObservableValue(randomValue)
            observableValues.Add(observableValue)
        { model with 
            Actions = model.Actions @ [ { Description = "Reset" } ]            
        }

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "Reset" |> Binding.cmd Reset
    "Series" |> Binding.oneWayLazy ((fun _ -> series), (fun _ _ -> true), id)
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)