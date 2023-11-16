namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type MainViewModel() as this =
    inherit ReactiveElmishViewModel<Model, Msg>(App.init())

    let counterVM = lazy (new CounterViewModel(this) : ReactiveUI.ReactiveObject)
    let aboutVM = lazy (new AboutViewModel(this))
    let chartVM = lazy (new ChartViewModel(this))
    let filePickerVM = lazy (new FilePickerViewModel())

    member this.ContentVM = this.Bind (fun m -> 
        match m.View with
        | CounterView -> counterVM.Value
        | AboutView -> aboutVM.Value
        | ChartView -> chartVM.Value
        | FilePickerView -> filePickerVM.Value
    )
    
    member this.ShowChart() = this.Dispatch (SetView ChartView)
    member this.ShowCounter() = this.Dispatch (SetView CounterView)
    member this.ShowAbout() = this.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = this.Dispatch (SetView FilePickerView)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple App.init App.update
        |> Program.withErrorHandler (fun (_, ex) -> printfn $"Error: {ex.Message}")
        |> Program.withConsoleTrace
        |> Program.runView this view
        
    static member DesignVM = new MainViewModel()