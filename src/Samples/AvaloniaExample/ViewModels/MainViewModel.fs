module AvaloniaExample.ViewModels.MainViewModel

open Elmish.Avalonia
open Elmish

type Model = 
    {
        ContentVM: IElmishViewModel
    }

type Msg = 
    | ShowChart
    | ShowCounter
    | ShowAbout

let init() = 
    { 
        ContentVM = ChartViewModel.vm
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | ShowChart -> 
        { model with ContentVM = ChartViewModel.vm }
    | ShowCounter -> 
        { model with ContentVM = CounterViewModel.vm }  
    | ShowAbout ->
        { model with ContentVM = AboutViewModel.vm }

let bindings() : Binding<Model, Msg> list = [ 
    // Properties
    "ContentVM" |> Binding.oneWay (fun m -> m.ContentVM)
    "ShowChart" |> Binding.cmd ShowChart
    "ShowCounter" |> Binding.cmd ShowCounter
    "ShowAbout" |> Binding.cmd ShowAbout
]

let designVM = ViewModel.designInstance (init()) (bindings())


let vm : IElmishViewModel = 
    let program =
        let subscriptions (model: Model) : Sub<Msg> =
            let messageBusSubscription (dispatch: Msg -> unit) = 
                Messaging.bus.Subscribe(fun msg -> 
                    match msg with
                    | Messaging.GlobalMsg.GoHome -> 
                        dispatch ShowChart
                )

            [ 
                [ nameof messageBusSubscription ], messageBusSubscription
            ]

        AvaloniaProgram.mkSimple init update bindings
        |> AvaloniaProgram.withSubscription subscriptions

    ElmishViewModel(program)