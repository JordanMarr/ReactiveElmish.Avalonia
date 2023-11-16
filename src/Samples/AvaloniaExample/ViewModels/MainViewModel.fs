namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type MainViewModel() as this =
    inherit ReactiveViewModel()

    let app = 
        Program.mkAvaloniaSimple App.init App.update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.mkStore
        |> this.AddDisposable

    let counterVM = lazy (new CounterViewModel(app) : ReactiveUI.ReactiveObject)
    let aboutVM = lazy (new AboutViewModel(app))
    let chartVM = lazy (new ChartViewModel(app))
    let filePickerVM = lazy (new FilePickerViewModel())

    member this.ContentVM = this.Bind (app, fun m -> 
        match m.View with
        | CounterView -> counterVM.Value
        | AboutView -> aboutVM.Value
        | ChartView -> chartVM.Value
        | FilePickerView -> filePickerVM.Value
    )
    
    member this.ShowChart() = app.Dispatch (SetView ChartView)
    member this.ShowCounter() = app.Dispatch (SetView CounterView)
    member this.ShowAbout() = app.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = app.Dispatch (SetView FilePickerView)
        
    static member DesignVM = new MainViewModel()