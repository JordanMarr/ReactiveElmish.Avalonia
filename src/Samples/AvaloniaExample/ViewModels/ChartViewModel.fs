module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open Elmish
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults
open Messaging

let rnd = Random()

let newSeries (count: int option)  =
    let newCollection = ObservableCollection<DateTimePoint>()
    // use seriesCount to either 1) set a default 15 at init or 2) use the count passed in from Reset 
    let mutable seriesCount = 0
    match count with
    | None ->
        seriesCount <- 15
    | _ -> 
        seriesCount <- count.Value - 1
    for i = seriesCount downto 0 do
        // backdate the time in seconds by the index to create a series of points in the past
        let past = DateTimeOffset.Now.AddSeconds(-i).LocalDateTime
        let randomNull = rnd.Next(0, 99)
        // in 1% of cases produce a null value to show an "empty" spot in the series
        match randomNull with
            | i when i = 0 ->
                newCollection.Add(DateTimePoint(past, System.Nullable()))
            | _ -> newCollection.Add(DateTimePoint(past, rnd.Next(0, 10)))
    newCollection
    
// create time labeling for the X axis in the Chart visual
let XAxes : IEnumerable<ICartesianAxis> =
    [| Axis (
            Labeler = (fun value -> DateTime(int64 value).ToString("HH:mm:ss")),
            LabelsRotation = 15,
            UnitWidth = float(TimeSpan.FromSeconds(1).Ticks),
            MinStep = float(TimeSpan.FromSeconds(1).Ticks)
        )
    |]

type Model = 
    {
        Series: ObservableCollection<ISeries>
        Actions: Action list
        IsAutoUpdateChecked: bool
    }
    
and Action = 
    {
        Description: string
        Timestamp: DateTime
    }

type Msg = 
    | AddItem
    | AddNull
    | RemoveItem
    | UpdateItem
    | ReplaceItem
    | Reset
    | SetIsAutoUpdateChecked of bool
    | Terminate
    | Ok

let init() =
    {
        Series = 
            ObservableCollection<ISeries> 
                [ 
                    ColumnSeries<DateTimePoint>(Values = newSeries(None), Name = "Luck By Second")
                    :> ISeries 
                ]
        Actions = [ { Description = "Initialized Chart"; Timestamp = DateTime.Now } ]
        IsAutoUpdateChecked = false
    }

let update (msg: Msg) (model: Model) =
    let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
    match msg with
    | AddItem ->
        values.Insert(values.Count, DateTimePoint(DateTime.Now, rnd.Next(0, 10)))
        { model with 
            Actions = model.Actions @ [ { Description = $"Added Item"; Timestamp = DateTime.Now } ]    
        }
    | AddNull ->
        values.Insert(values.Count, DateTimePoint(DateTime.Now, System.Nullable()))
        { model with 
            Actions = model.Actions @ [ { Description = $"Added Null"; Timestamp = DateTime.Now } ]    
        }
    | RemoveItem ->
        values.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "Removed Item"; Timestamp = DateTime.Now } ]    
        }
    | UpdateItem ->
        let item = rnd.Next(0, values.Count - 1)
        let fstValueTime = values[item].DateTime
        values[item] <- DateTimePoint(fstValueTime, rnd.Next(0, 10))
        { model with 
            Actions = model.Actions @ [ { Description = $"Updated Item: {item + 1}"; Timestamp = DateTime.Now } ]            
        }
    | ReplaceItem ->
        let lastValueTime = values[values.Count - 1].DateTime
        values[values.Count - 1] <- DateTimePoint(lastValueTime, rnd.Next(0, 10))
        { model with 
            Actions = model.Actions @ [ { Description = $"Replaced Item: {values.Count}"; Timestamp = DateTime.Now } ]           
        }
    | Reset ->
        // insert new Series - send the current series length to the newSeries function
        model.Series[0].Values <- newSeries(Some values.Count)
        { model with
            // deactivate the AutoUpdate ToggleButton in the UI
            IsAutoUpdateChecked = false 
            Actions = [ { Description = "Reset Chart"; Timestamp = DateTime.Now } ]
        }
    | SetIsAutoUpdateChecked isChecked ->
        { model with 
            IsAutoUpdateChecked = isChecked
            Actions = model.Actions @ [ { Description = $"Is AutoUpdate Checked: {isChecked}"; Timestamp = DateTime.Now } ]
        }
    | Ok -> 
        bus.OnNext(GlobalMsg.GoHome)
        { model with IsAutoUpdateChecked = false }
    | Terminate ->
        model

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> List.rev m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "Reset" |> Binding.cmd Reset
    "IsAutoUpdateChecked" |> Binding.twoWay ((fun m -> m.IsAutoUpdateChecked), SetIsAutoUpdateChecked)
    "Series" |> Binding.oneWayLazy ((fun m -> m.Series), (fun _ _ -> true), id)
    "XAxes" |> Binding.oneWayLazy ((fun _ -> XAxes), (fun _ _ -> true), id)
    "Ok" |> Binding.cmd Ok
]

let designVM = ViewModel.designInstance (init()) (bindings())

open System.Timers

let subscriptions (model: Model) : Sub<Msg> =
    let autoUpdateSub (dispatch: Msg -> unit) = 
        let timer = new Timer(1000) 
        let disposable = 
            timer.Elapsed.Subscribe(fun _ -> 
                // similar to newSeries create null entry in 1% of cases
                let randomNull = rnd.Next(0, 99)
                match randomNull with
                | i when i = 0 -> 
                    dispatch AddNull
                | _ -> 
                    dispatch AddItem
                dispatch RemoveItem
            )
        timer.Start()
        disposable

    [
        if model.IsAutoUpdateChecked then
            [ nameof autoUpdateSub ], autoUpdateSub
    ]


let vm = 
    AvaloniaProgram.mkSimple init update bindings
    |> AvaloniaProgram.withSubscription subscriptions
    |> ElmishViewModel.create
    |> ElmishViewModel.terminateOnUnloaded Terminate

