module AvaloniaExample.ViewModels.ChartViewModel

open System
open Elmish.Avalonia
open LiveChartsCore
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
    | RemoveItem
    | UpdateItem
    | ReplaceItem

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
    | RemoveItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "RemoveItem" } ]            
            Data = model.Data |> List.rev |> List.tail |> List.rev
        }
    | UpdateItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "UpdateItem" } ]            
            Data = model.Data |> List.rev |> List.tail |> List.rev |> List.map (fun x -> x + 1)
        }
    | ReplaceItem ->
        { model with 
            Actions = model.Actions @ [ { Description = "ReplaceItem" } ]            
            Data = model.Data |> List.rev |> List.tail |> List.rev |> List.map (fun x -> _random.Next(0, 10))
        }

let bindings ()  : Binding<Model, Msg> list = [
    "Actions" |> Binding.oneWay (fun m -> m.Actions)
    "AddItem" |> Binding.cmd AddItem
    "RemoveItem" |> Binding.cmd RemoveItem
    "UpdateItem" |> Binding.cmd UpdateItem
    "ReplaceItem" |> Binding.cmd ReplaceItem
    "Series" |> Binding.oneWay (fun m -> 
        [|
            LineSeries<int>(Values = m.Data, Fill = null, Name = "Income") :> ISeries
        |]
    )
]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkSimple init update bindings)