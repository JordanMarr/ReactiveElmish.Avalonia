namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish

module Main = 

    type Model = 
        {
            ContentVM: IElmishViewModel
        }

    type Msg = 
        | ShowChart
        | ShowCounter
        | ShowAbout
        | ShowFilePicker
        | Terminate

    let init() = 
        { 
            ContentVM = new CounterViewModel()
        }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | ShowCounter -> 
            { ContentVM = new CounterViewModel() }
        | ShowChart -> 
            { ContentVM = new ChartViewModel() }  
        | ShowAbout ->
            { ContentVM = new AboutViewModel() }
        | ShowFilePicker ->
            { ContentVM = new FilePickerViewModel() }
        | Terminate ->
            model

    let subscriptions (model: Model) : Sub<Msg> =
        let messageBusSub (dispatch: Msg -> unit) = 
            Messaging.bus.Subscribe(fun msg -> 
                match msg with
                | Messaging.GlobalMsg.GoHome -> 
                    dispatch ShowCounter
            )

        [ 
            [ nameof messageBusSub ], messageBusSub
        ]

open Main

type MainViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init())

    member this.ContentVM = this.Bind _.ContentVM
    member this.ShowChart() = this.Dispatch ShowChart
    member this.ShowCounter() = this.Dispatch ShowCounter
    member this.ShowAbout() = this.Dispatch ShowAbout
    member this.ShowFilePicker() = this.Dispatch ShowFilePicker

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.withSubscription subscriptions
        |> Program.terminateOnViewUnloaded this Terminate
        |> Program.runView this view

    static member DesignVM = new MainViewModel()