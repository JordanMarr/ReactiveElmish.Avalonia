namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Avalonia.Controls
open AvaloniaExample
open App

type MainViewModel() =
    inherit ReactiveElmishViewModel()

    let app = 
        Program.mkAvaloniaSimple App.init App.update
        |> Program.withErrorHandler (fun (_, ex) -> printfn $"Error: {ex.Message}")
        //|> Program.withConsoleTrace
        |> Program.mkStore

    let counterView = lazy (ViewBinder.bindSingleton (new CounterViewModel(app), Views.CounterView()) |> snd)
    let aboutView = lazy (ViewBinder.bindSingleton (new AboutViewModel(app), Views.AboutView()) |> snd)
    let chartView = lazy (ViewBinder.bindSingleton (new ChartViewModel(app), Views.ChartView()) |> snd)
    let filePickerView = lazy (ViewBinder.bindSingleton (new FilePickerViewModel(app), Views.FilePickerView()) |> snd)

    member this.ContentView = 
        this.Bind (app, fun m -> 
            match m.View with
            | CounterView -> counterView.Value
            | AboutView -> aboutView.Value
            | ChartView -> chartView.Value
            | FilePickerView -> filePickerView.Value
            : Control
        )
    
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = 
        new MainViewModel()