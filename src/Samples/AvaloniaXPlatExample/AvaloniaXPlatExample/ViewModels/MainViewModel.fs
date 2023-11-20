namespace AvaloniaXPlatExample.ViewModels

open Elmish.Avalonia
open Elmish
open AvaloniaXPlatExample.ViewModels
open Avalonia.Controls
open App

type MainViewModel() =
    inherit ReactiveElmishViewModel()

    member this.CounterView = this.GetView<CounterViewModel>()
    member this.AboutView = this.GetView<AboutViewModel>()
    member this.ChartView = this.GetView<ChartViewModel>()
    member this.FilePickerView = this.GetView<FilePickerViewModel>()

    //member this.ContentView : Control = 
    //    this.Bind (app, fun m -> 
    //        match m.View with
    //        | CounterView -> this.GetView<CounterViewModel>()
    //        | AboutView -> this.GetView<AboutViewModel>()
    //        | ChartView -> this.GetView<ChartViewModel>()
    //        | FilePickerView -> this.GetView<FilePickerViewModel>()
            
    //    )
    
    //member this.ShowChart() = app.Dispatch (SetView ChartView)
    //member this.ShowCounter() = app.Dispatch (SetView CounterView)
    //member this.ShowAbout() = app.Dispatch (SetView AboutView)
    //member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = 
        new MainViewModel()