namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type MainViewModel() as this =
    inherit ReactiveElmishViewModel<Model, Msg>(App.init())

    let counterVM = new CounterViewModel(this)
    let aboutVM = new AboutViewModel(this)

    member this.ContentVM = this.Bind (fun m -> 
        match m.View with
        | CounterView -> counterVM
        | AboutView -> aboutVM
        | ChartView -> new ChartViewModel(this)
        | FilePickerView -> new FilePickerViewModel(this)
        : ReactiveUI.ReactiveObject
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