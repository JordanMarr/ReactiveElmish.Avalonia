module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Runtime.Serialization
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let _random = Random()
  
let newSeries =
    let newCollection = ObservableCollection<DateTimePoint>()
    for i = 10 downto 0 do
        let now = DateTimeOffset.Now
        let past = now.AddSeconds(-i).LocalDateTime
        newCollection.Add(DateTimePoint(past, _random.Next(1, 11)))
    newCollection
    
let XAxes : IEnumerable<ICartesianAxis> =
    [| Axis (
            Labeler = (fun value -> DateTime(int64 value).ToString("hh:mm:ss")),
            LabelsRotation = 15,
            UnitWidth = float(TimeSpan.FromSeconds(1).Ticks),
            MinStep = float(TimeSpan.FromSeconds(1).Ticks)
        )
    |]

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
    | AutoUpdate

let rec init() =
    {
        Series = 
            ObservableCollection<ISeries> 
                [ 
                    LineSeries<DateTimePoint>(Values = newSeries, Fill = null, Name = "Luck By Second") :> ISeries 
                ]
        Actions = [ { Description = "Initialized"} ]
    }
    
let mutable isAutoUpdating = false
let update (msg: Msg) (model: Model) =
    match msg with
    | AddItem ->
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        values.Insert(values.Count, (DateTimePoint(DateTime.Now, _random.Next(1, 11))))
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]    
        }
    | RemoveItem ->
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        values.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]    
        }
    | UpdateItem ->
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        let item = _random.Next(0, values.Count)
        let fstValueTime = values.[item].DateTime
        values[item] <- DateTimePoint(fstValueTime, _random.Next(1, 11))
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
        }
    | ReplaceItem ->
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        let lastValueTime = values[values.Count - 1].DateTime
        values[0] <- DateTimePoint(lastValueTime, _random.Next(1, 11))
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
        }
    | Reset ->
        // I do not know why I can't just use newSeries here
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        let newCollection = ObservableCollection<DateTimePoint>()
        for i = values.Count downto 0 do
            let now = DateTimeOffset.Now
            let past = now.AddSeconds(-i).LocalDateTime
            newCollection.Add(DateTimePoint(past, _random.Next(1, 11)))
        model.Series[0].Values <- newCollection
        // end newSeries complaint
        { model with 
            Actions = model.Actions @ [ { Description = "Reset" } ]            
        }
    | AutoUpdate ->
        match isAutoUpdating with
            | false ->
                isAutoUpdating <- true
                { model with 
                    Actions = model.Actions @ [ { Description = "Continue" } ]            
                }
            | _ ->
                isAutoUpdating <- false
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
    "AutoUpdate" |> Binding.cmd AutoUpdate
    "Series" |> Binding.oneWayLazy ((fun m -> m.Series), (fun _ _ -> true), id)
    "XAxes" |> Binding.oneWayLazy ((fun _ -> XAxes), (fun _ _ -> true), id)
]

let designVM = ViewModel.designInstance (init()) (bindings())

open Elmish
open System.Timers

let subscriptions (model: Model) : Sub<Msg> =

    let valueChangedSubscription (dispatch: Msg -> unit) = 
        let timer = new Timer(1000) 
        timer.Elapsed.Add(fun _ -> 
            if isAutoUpdating then
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