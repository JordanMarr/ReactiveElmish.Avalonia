namespace AvaloniaExample.ViewModels

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Reactive.Linq
open ReactiveElmish
open ReactiveElmish.Avalonia
open Elmish
open DynamicData
open LiveChartsCore
open LiveChartsCore.Kernel.Sketches
open LiveChartsCore.SkiaSharpView
open LiveChartsCore.Defaults

module Chart = 

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
            Actions: SourceList<Action>
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
    
    let init() =
        {
            Series = ObservableCollection<ISeries> 
                [ 
                    ColumnSeries<DateTimePoint>(Values = newSeries(None), Name = "Luck By Second") :> ISeries 
                ]
            Actions = SourceList.createFrom [ { Description = "Initialized Chart"; Timestamp = DateTime.Now }]
            IsAutoUpdateChecked = false
        }

    let update (msg: Msg) (model: Model) =
        let values = model.Series[0].Values :?> ObservableCollection<DateTimePoint>
        match msg with
        | AddItem ->
            values.Insert(values.Count, DateTimePoint(DateTime.Now, rnd.Next(0, 10)))
            { model with 
                Actions = model.Actions |> SourceList.add { Description = $"Added Item"; Timestamp = DateTime.Now } }
        | AddNull ->
            values.Insert(values.Count, DateTimePoint(DateTime.Now, System.Nullable()))
            { model with 
                Actions = model.Actions |> SourceList.add { Description = $"Added Null"; Timestamp = DateTime.Now } }
        | RemoveItem ->
            values.RemoveAt(0)
            { model with 
                Actions = model.Actions |> SourceList.add { Description = $"Removed Item"; Timestamp = DateTime.Now } }
        | UpdateItem ->
            let item = rnd.Next(0, values.Count - 1)
            let fstValueTime = values[item].DateTime
            values[item] <- DateTimePoint(fstValueTime, rnd.Next(0, 10))
            { model with 
                Actions = model.Actions |> SourceList.add { Description = $"Updated Item: {item + 1}"; Timestamp = DateTime.Now } }
        | ReplaceItem ->
            let lastValueTime = values[values.Count - 1].DateTime
            values[values.Count - 1] <- DateTimePoint(lastValueTime, rnd.Next(0, 10))
            { model with 
                Actions = model.Actions |> SourceList.add { Description = $"Replaced Item: {values.Count}"; Timestamp = DateTime.Now } }
        | Reset ->
            // insert new Series - send the current series length to the newSeries function
            model.Series[0].Values <- newSeries(Some values.Count)
            { model with
                // deactivate the AutoUpdate ToggleButton in the UI
                IsAutoUpdateChecked = false 
                Actions = model.Actions |> SourceList.removeAll |> SourceList.add { Description = "Reset Chart"; Timestamp = DateTime.Now }
            }
        | SetIsAutoUpdateChecked isChecked ->
            { model with 
                IsAutoUpdateChecked = isChecked
                Actions = model.Actions |> SourceList.add { Description = $"Is AutoUpdate Checked: {isChecked}"; Timestamp = DateTime.Now }
            }
        | Terminate ->
            model

    let subscriptions (model: Model) : Sub<Msg> =
        let autoUpdateSub (dispatch: Msg -> unit) = 
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(fun _ -> 
                    printfn "AutoUpdate"
                    // similar to newSeries create null entry in 1% of cases
                    let randomNull = rnd.Next(0, 99)
                    match randomNull with
                    | i when i = 0 -> 
                        dispatch AddNull
                    | _ -> 
                        dispatch AddItem
                    dispatch RemoveItem
                )

        [
            if model.IsAutoUpdateChecked then
                [ nameof autoUpdateSub ], autoUpdateSub
        ]

open Chart

type ChartViewModel() as this =
    inherit ReactiveElmishViewModel()

    let app = App.app

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        //|> Program.withConsoleTrace
        |> Program.withSubscription subscriptions
        |> Program.mkStore
        //Terminate all Elmish subscriptions on dispose (view is registered as Transient).
        //|> Program.mkStoreWithTerminate this Terminate 

    do  // Manually disable AutoUpdate (when view is registered as Singleton).
        this.Subscribe(app.Observable, fun m -> 
            if m.View <> App.ChartView && this.IsAutoUpdateChecked then
                printfn "Disabling Chart AutoUpdate"
                local.Dispatch (SetIsAutoUpdateChecked false)
        )

    member this.Series = local.Model.Series
    member this.Actions = this.BindSourceList(local.Model.Actions)
    member this.AddItem() = local.Dispatch AddItem
    member this.RemoveItem() = local.Dispatch RemoveItem
    member this.UpdateItem() = local.Dispatch UpdateItem
    member this.ReplaceItem() = local.Dispatch ReplaceItem
    member this.Reset() = local.Dispatch Reset
    member this.IsAutoUpdateChecked 
        with get () = this.Bind (local, _.IsAutoUpdateChecked)
        and set value = local.Dispatch (SetIsAutoUpdateChecked value)
    member this.XAxes = this.Bind (local, fun _ -> XAxes)
    member this.Ok() = app.Dispatch App.GoHome

    static member DesignVM = new ChartViewModel()