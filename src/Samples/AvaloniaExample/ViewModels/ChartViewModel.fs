module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.Defaults
open LiveChartsCore.SkiaSharpView

let _random = Random()

let _observableValues = ObservableCollection<ObservableValue>([
        ObservableValue(2)
        ObservableValue(5)
        ObservableValue(4)
        ObservableValue(5)
        ObservableValue(2)
        ObservableValue(6)
        ObservableValue(6)
        ObservableValue(6)
        ObservableValue(4)
        ObservableValue(2)
        ObservableValue(3)
        ObservableValue(4)
        ObservableValue(3)
    ])

type ChartData() =
    member val collection = ObservableCollection<ISeries>() with get, set
let Series =
    let lineSeries =
        LineSeries<ObservableValue>(Values = _observableValues, Fill = null) :> ISeries
    let series =
        seq {
            lineSeries
        } 
        |> ObservableCollection<_>
    ChartData(collection = series)
    
// let Series =
//         seq {
//             LineSeries<ObservableValue>(Values = _observableValues, Fill = null) :> ISeries
//         } 
//         |> ObservableCollection<_>
      

let AddItem() =
    _observableValues.Add(ObservableValue(_random.Next(0, 10)))
    

let RemoveItem() =
    _observableValues.RemoveAt(_observableValues.Count - 1)


let UpdateItem() =
    _observableValues.[_observableValues.Count - 1] <- ObservableValue(_random.Next(0, 10))


let ReplaceItem() =
    _observableValues.[_observableValues.Count - 1] <- ObservableValue(_random.Next(0, 10))


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

let init() = 
    { 
        Actions = [ { Description = "Initialized count."} ]
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | AddItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]
        }
    | RemoveItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]
        }
    | UpdateItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]
        }
    | ReplaceItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]
        }


let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)