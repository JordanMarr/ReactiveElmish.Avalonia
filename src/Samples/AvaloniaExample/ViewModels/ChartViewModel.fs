module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.ObjectModel
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let _random = Random()
  
let newSeries =
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
    | Reset
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
    
let mutable isContinuing = false
let update (msg: Msg) (model: Model) =
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
    | Reset ->
        // I do not know why I can't just use newSeries here
        let values = model.Series[0].Values :?> ObservableCollection<ObservableValue>
        let newCollection = ObservableCollection<ObservableValue>()
        for _ in 1 .. values.Count - 1 do
            newCollection.Add(ObservableValue(_random.Next(1, 11)))
        model.Series[0].Values <- newCollection
        // end newSeries complaint
        { model with 
            Actions = model.Actions @ [ { Description = "Reset" } ]            
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
    "Reset" |> Binding.cmd Reset
    "Continue" |> Binding.cmd Continue
    "Series" |> Binding.oneWayLazy ((fun m -> m.Series), (fun _ _ -> true), id)
]

let designVM = ViewModel.designInstance (init()) (bindings())

open Elmish
open System.Timers

let subscriptions (model: Model) : Sub<Msg> =

    let valueChangedSubscription (dispatch: Msg -> unit) = 
        let timer = new Timer(1000) 
        timer.Elapsed.Add(fun _ -> 
            if isContinuing then
                dispatch AddItem
                dispatch RemoveItem
        )
        timer.Start()
        timer :> IDisposable

    [
        [ nameof valueChangedSubscription ], valueChangedSubscription
    ]

let vm = ElmishViewModel(
    AvaloniaProgram.mkSimple init update bindings
    |> AvaloniaProgram.withSubscription subscriptions
)