namespace AvaloniaXPlatExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open Elmish
open AvaloniaXPlatExample.ViewModels
open Avalonia.Controls
open App

type MainViewModel(root: CompositionRoot) =
    inherit ReactiveElmishViewModel()

    member this.SelectedTabIndex
        with get () =
            this.Bind(app, fun m ->
                match m.View with
                | CounterView -> 0
                | ChartView -> 1
                | FilePickerView -> 2
                | ListBoxView -> 3
                | AboutView -> 4)
        and set value =
            match value with
            | 0 -> app.Dispatch (SetView CounterView)
            | 1 -> app.Dispatch (SetView ChartView)
            | 2 -> app.Dispatch (SetView FilePickerView)
            | 3 -> app.Dispatch (SetView ListBoxView)
            | 4 -> app.Dispatch (SetView AboutView)
            | _ -> ()

    member this.CounterView = root.GetView<CounterViewModel>()
    member this.AboutView = root.GetView<AboutViewModel>()
    member this.ChartView = root.GetView<ChartViewModel>()
    member this.FilePickerView = root.GetView<FilePickerViewModel>()
    member this.ListBoxView = root.GetView<ListBoxViewModel>()

    static member DesignVM = new MainViewModel(Design.stub)
