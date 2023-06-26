module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open System.Threading.Tasks
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let _random = Random()
  
let newSeries : ObservableCollection<ObservableValue> =
    let newCollection = ObservableCollection<ObservableValue>()
    for _ in 1 .. 10 do
        newCollection.Add(ObservableValue(_random.Next(1, 11)))
    newCollection
    

type Model = 
    {
        Series: ObservableCollection<ISeries> 
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
    | Continue

let rec init() =
    {
        Series = 
            ObservableCollection<ISeries> 
                [ 
                    LineSeries<ObservableValue>(Values = newSeries) :> ISeries 
                ]
        Actions = [ { Description = "Initialized"} ]
    }
    

let update (msg: Msg) (model: Model) =
    // new section
    let mutable isContinuing = false
    let runContinuous () =
        async {
            let rec streamingLoop () =
                if isContinuing then
                    let values = model.Series.[0].Values :?> ObservableCollection<ObservableValue>
                    values.RemoveAt(0)
                    values.Add(ObservableValue(_random.Next(1, 11)))
                    Task.Delay(1000) |> Async.AwaitTask |> ignore
                    streamingLoop ()

            streamingLoop ()
        }
    runContinuous () |> Async.StartImmediate
    // end new section
    
    match msg with
    | AddItem ->
        let values = model.Series[0].Values :?> ObservableCollection<ObservableValue>
        values.Add(ObservableValue(_random.Next(1, 11)))
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]    
        }
    | RemoveItem ->
        let values = model.Series[0].Values :?> ObservableCollection<ObservableValue>
        values.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]    
        }
    | UpdateItem ->
        let values = model.Series[0].Values :?> ObservableCollection<ObservableValue>
        let len = values.Count
        let item = _random.Next(0, len)
        values[item] <- ObservableValue(_random.Next(1, 11))
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
        }
    | ReplaceItem ->
        let values = model.Series[0].Values :?> ObservableCollection<ObservableValue>
        let lstValue = values.Count-1
        values[lstValue] <- ObservableValue(_random.Next(1, 11))
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
        }
    | Continue ->
        match isContinuing with
            | false ->
                isContinuing <- true
                { model with 
                    Actions = model.Actions @ [ { Description = "Continue" } ]            
                }
            | _ ->
                isContinuing <- false
                { model with 
                    Actions = model.Actions @ [ { Description = "Continue" } ]            
                } 

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "Continue" |> Binding.cmd Continue
    "Series" |> Binding.oneWayLazy ((fun m -> m.Series), (fun _ _ -> true), id)
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)