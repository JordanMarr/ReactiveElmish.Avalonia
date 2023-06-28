module AvaloniaExample.ViewModels.ChartViewModel

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open Elmish.Avalonia
open LiveChartsCore
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

let rnd = Random()

  
let newSeries (count: int option)  =
    let newCollection = ObservableCollection<DateTimePoint>()
    // use seriesCount to either 1) set a default 15 at init or 2) use the count passed in from the reset button
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
    }

type Msg = 
    | AddItem
    | AddNull
    | RemoveItem
    | UpdateItem
    | ReplaceItem
    | Reset
    | AutoUpdate
    | SetIsAutoUpdateChecked of bool

let rec init() =
    {
        Series = 
            ObservableCollection<ISeries> 
                [ 
                    LineSeries<DateTimePoint>(Values = newSeries(None), Fill = null, Name = "Luck By Second") :> ISeries 
                ]
        Actions = [ { Description = "Initialized"} ]
        IsAutoUpdateChecked = false 
    }
    
let mutable isAutoUpdating = false
let update (msg: Msg) (model: Model) =
    let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
    match msg with
    | AddItem ->
        values.Insert(values.Count, (DateTimePoint(DateTime.Now, rnd.Next(0, 10))))
        { model with 
            Actions = model.Actions @ [ { Description = "AddItem" } ]    
        }
    | AddNull ->
        values.Insert(values.Count, (DateTimePoint(DateTime.Now, System.Nullable())))
        { model with 
            Actions = model.Actions @ [ { Description = "AddNull" } ]    
        }
    | RemoveItem ->
        values.RemoveAt(0)
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]    
        }
    | UpdateItem ->
        let item = rnd.Next(0, values.Count - 1)
        let fstValueTime = values.[item].DateTime
        values[item] <- DateTimePoint(fstValueTime, rnd.Next(0, 10))
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
        }
    | ReplaceItem ->
        let lastValueTime = values[values.Count - 1].DateTime
        values[values.Count - 1] <- DateTimePoint(lastValueTime, rnd.Next(0, 10))
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
        }
    | Reset ->
        // pass up the current length of the series to the newSeries function
        isAutoUpdating <- false
        model.Series[0].Values <- newSeries(Some values.Count)
        { model with 
            IsAutoUpdateChecked = false 
            Actions = model.Actions @ [ { Description = "Reset" } ]
        }
    | SetIsAutoUpdateChecked isChecked ->
        { model with 
            IsAutoUpdateChecked = isChecked
            Actions = model.Actions @ [ { Description = $"IsAutoUpdateChecked: {isChecked}" } ]
        }
    | AutoUpdate ->
        match isAutoUpdating with
            | false ->
                isAutoUpdating <- true
                { model with 
                    Actions = model.Actions @ [ { Description = "AutoUpdate" } ]
                    IsAutoUpdateChecked = true 
                }
            | _ ->
                isAutoUpdating <- false
                { model with 
                    Actions = model.Actions @ [ { Description = "AutoUpdate" } ]
                    IsAutoUpdateChecked = false 
                }

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "IsAutoUpdateChecked" |> Binding.twoWay ((fun m -> m.IsAutoUpdateChecked), SetIsAutoUpdateChecked)
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
                // similar to newSeries create null entries in 1% of cases
                let _randomNull = rnd.Next(0, 99)
                match _randomNull with
                | i when i = 0 ->
                    dispatch AddNull
                | _ -> dispatch AddItem
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