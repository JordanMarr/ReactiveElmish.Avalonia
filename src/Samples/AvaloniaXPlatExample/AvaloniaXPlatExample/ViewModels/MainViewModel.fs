namespace AvaloniaXPlatExample.ViewModels

open Elmish.Avalonia
open Elmish
open AvaloniaXPlatExample.ViewModels
open Avalonia.Controls
open App

type MainViewModel() =
    inherit ReactiveElmishViewModel()

    member this.SelectedTabIndex 
        with get () = 
            this.Bind(app, fun m -> 
                match m.View with
                | CounterView -> 0
                | ChartView -> 1
                | FilePickerView -> 2
                | AboutView -> 3)
        and set value = 
            match value with
            | 0 -> app.Dispatch (SetView CounterView)
            | 1 -> app.Dispatch (SetView ChartView)
            | 2 -> app.Dispatch (SetView FilePickerView)
            | 3 -> app.Dispatch (SetView AboutView)
            | _ -> ()

    member this.CounterView = this.GetView<CounterViewModel>()
    member this.AboutView = this.GetView<AboutViewModel>()
    member this.ChartView = this.GetView<ChartViewModel>()
    member this.FilePickerView = this.GetView<FilePickerViewModel>()

    static member DesignVM = new MainViewModel()