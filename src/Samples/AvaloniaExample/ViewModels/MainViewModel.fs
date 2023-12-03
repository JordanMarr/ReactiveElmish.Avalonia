namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open App

type MainViewModel(root: CompositionRoot) =
    inherit ReactiveElmishViewModel()
    
    member this.ContentView = 
        this.BindOnChanged (app, _.View, fun m -> 
            match m.View with
            | TodoListView -> root.GetView<TodoListViewModel>()
            | CounterView -> root.GetView<CounterViewModel>()
            | AboutView -> root.GetView<AboutViewModel>()
            | ChartView -> root.GetView<ChartViewModel>()
            | FilePickerView -> root.GetView<FilePickerViewModel>()
        )

    member this.ShowTodoList() = app.Dispatch (SetView TodoListView)
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = new MainViewModel(Design.stub)