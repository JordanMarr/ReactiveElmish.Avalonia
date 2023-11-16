namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type MainViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init())

    member this.ContentVM = this.Bind (fun m -> 
        match m.View with
        | CounterView -> new CounterViewModel(this) : IElmishViewModel
        | AboutView -> new AboutViewModel(this)
        | ChartView -> new ChartViewModel()
        | FilePickerView -> new FilePickerViewModel()
    )
    
    member this.ShowChart() = this.Dispatch (SetView ChartView)
    member this.ShowCounter() = this.Dispatch (SetView CounterView)
    member this.ShowAbout() = this.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = this.Dispatch (SetView FilePickerView)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

    static member DesignVM = new MainViewModel()