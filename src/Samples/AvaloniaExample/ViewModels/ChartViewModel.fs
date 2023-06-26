module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let _random = Random()

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

let observableValues = ObservableCollection<ObservableValue>()
let series = 
    ObservableCollection<ISeries> 
        [ 
            ColumnSeries<ObservableValue>(Values = observableValues) :> ISeries 
        ]
       
        
let replaceItemAtPosition (position: int) (newValue: ObservableValue) =
    if position >= 0 && position < observableValues.Count then
        observableValues.[position] <- newValue

let init() =
    for _ in 1 .. 5 do
        let randomValue = _random.Next(1, 10)
        let observableValue = new ObservableValue(randomValue)
        observableValues.Add(observableValue)
    { 
        Actions = [ { Description = "AddItem"} ]
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | AddItem ->
        observableValues.Add(ObservableValue(_random.Next(0, 10)))
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]    
        }
    | RemoveItem ->
        observableValues.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]    
        }
    | UpdateItem ->
        replaceItemAtPosition (_random.Next(0, observableValues.Count-1)) (ObservableValue(_random.Next(0, 10)))
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
        }
    | ReplaceItem ->
        observableValues.RemoveAt(observableValues.Count-1)
        observableValues.Add(ObservableValue(_random.Next(0, 10)))
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
        }

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "Series" |> Binding.oneWayLazy ((fun m -> series), (fun a b -> true), id)
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)