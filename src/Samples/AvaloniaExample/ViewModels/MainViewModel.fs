namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Avalonia.Controls
open AvaloniaExample
open App

type MainViewModel() =
    inherit ReactiveElmishViewModel()

    member this.ContentView = 
        this.Bind (app, fun m -> 
            match m.View with
            | CounterView -> this.GetView<CounterViewModel>()
            | AboutView -> this.GetView<AboutViewModel>()
            | ChartView -> this.GetView<ChartViewModel>()
            | FilePickerView -> this.GetView<FilePickerViewModel>()
            : Control
        )
    
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = 
        new MainViewModel()