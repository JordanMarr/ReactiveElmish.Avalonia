namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open App

type MainViewModel(getView: CompositionRoot.GetView) =
    inherit ReactiveElmishViewModel()
    
    member this.ContentView = 
        this.BindOnChanged (app, _.View, fun m -> 
            match m.View with
            | TodoListView -> getView (typeof<TodoListViewModel>)
            | CounterView -> getView (typeof<CounterViewModel>)
            | AboutView -> getView (typeof<AboutViewModel>)
            | ChartView -> getView (typeof<ChartViewModel>)
            | FilePickerView -> getView (typeof<FilePickerViewModel>)
        )

    member this.ShowTodoList() = app.Dispatch (SetView TodoListView)
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = 
        let getViewStub = fun _ -> new AvaloniaExample.Views.CounterView() :> Avalonia.Controls.Control
        new MainViewModel(getViewStub)