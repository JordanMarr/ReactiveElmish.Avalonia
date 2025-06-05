namespace AvaloniaExample.ViewModels

open ReactiveElmish
open App

type IMainViewFacade = 
    abstract member GetView<'ViewModel & #ReactiveElmishViewModel>: unit -> Avalonia.Controls.Control

type MainViewModel(facade: IMainViewFacade) =
    inherit ReactiveElmishViewModel()
    
    member this.ContentView = 
        this.BindOnChanged (app, _.View, fun m -> 
            match m.View with
            | TodoListView -> facade.GetView<TodoListViewModel>()
            | CounterView -> facade.GetView<CounterViewModel>()
            | AboutView -> facade.GetView<AboutViewModel>()
            | ChartView -> facade.GetView<ChartViewModel>()
            | FilePickerView -> facade.GetView<FilePickerViewModel>()
        )

    member this.ShowTodoList() = app.Dispatch (SetView TodoListView)
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)

    static member DesignVM = 
        let designFacade = 
            { new IMainViewFacade with
                member this.GetView() = new AvaloniaExample.Views.CounterView() }

        new MainViewModel(designFacade)
