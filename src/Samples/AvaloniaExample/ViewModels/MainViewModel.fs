﻿namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type MainViewModel() =
    inherit ReactiveViewModel()

    let app = 
        Program.mkAvaloniaSimple App.init App.update
        |> Program.withErrorHandler (fun (_, ex) -> printfn $"Error: {ex.Message}")
        |> Program.withConsoleTrace
        |> Program.mkStore

    let counterVM = new CounterViewModel(app)
    let aboutVM = new AboutViewModel(app)

    member this.ContentVM = 
        this.Bind (app, fun m -> 
            match m.View with
            | CounterView -> counterVM
            | AboutView -> aboutVM
            | ChartView -> new ChartViewModel(app)
            | FilePickerView -> new FilePickerViewModel(app)
            : ReactiveUI.ReactiveObject
        )
    
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = new MainViewModel()