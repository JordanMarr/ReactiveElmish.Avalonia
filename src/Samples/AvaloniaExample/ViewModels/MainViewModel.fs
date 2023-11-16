namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish

module Main = 

    type Model = 
        {
            View: View
        }

    and View = 
    | CounterView
    | ChartView
    | AboutView
    | FilePickerView

    type Msg = 
        | SetView of View
        | Terminate

    let init() = 
        { 
            View = CounterView
        }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | SetView view -> 
            { View = view }
        | Terminate ->
            model

    let subscriptions (model: Model) : Sub<Msg> =
        let messageBusSub (dispatch: Msg -> unit) = 
            Messaging.bus.Subscribe(fun msg -> 
                match msg with
                | Messaging.GlobalMsg.GoHome -> 
                    dispatch (SetView CounterView)
            )

        [ 
            [ nameof messageBusSub ], messageBusSub
        ]

open Main

type MainViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init())

    member this.ContentVM = 
        this.Bind (fun m -> 
            match m.View with
            | CounterView -> new CounterViewModel() 
            | AboutView -> new AboutViewModel()
            | ChartView -> new ChartViewModel()
            | FilePickerView -> new FilePickerViewModel()
            : IElmishViewModel)

    member this.ShowChart() = this.Dispatch (SetView ChartView)
    member this.ShowCounter() = this.Dispatch (SetView CounterView)
    member this.ShowAbout() = this.Dispatch (SetView AboutView)
    member this.ShowFilePicker() = this.Dispatch (SetView FilePickerView)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.withSubscription subscriptions
        |> Program.terminateOnViewUnloaded this Terminate
        |> Program.runView this view

    static member DesignVM = new MainViewModel()