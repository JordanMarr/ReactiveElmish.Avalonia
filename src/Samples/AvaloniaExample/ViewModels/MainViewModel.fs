module AvaloniaExample.ViewModels.MainViewModel

open Elmish.Avalonia
open Elmish

type Model = 
    {
        ContentVM: IElmishViewModel
    }

type Msg = 
    | ShowCounter
    | ShowAbout

let init() = 
    { 
        ContentVM = CounterViewModel.vm
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | ShowCounter -> 
        { model with ContentVM = CounterViewModel.vm }
    | ShowAbout ->
        { model with ContentVM = AboutViewModel.vm }

let bindings() : Binding<Model, Msg> list = [ 
    // Properties
    "ContentVM" |> Binding.oneWay (fun m -> m.ContentVM)
    "ShowCounter" |> Binding.cmdIf (ShowCounter, fun m -> m.ContentVM <> CounterViewModel.vm)
    "ShowAbout" |> Binding.cmdIf (ShowAbout, fun m -> m.ContentVM <> AboutViewModel.vm)
]

let designVM = ViewModel.designInstance (init()) (bindings())


let vm : IElmishViewModel = 
    let program =
        let subscriptions (model: Model) : Sub<Msg> =
            let messageBusSubscription (dispatch: Msg -> unit) = 
                Messaging.bus.Subscribe(fun msg -> 
                    match msg with
                    | Messaging.GlobalMsg.GoHome -> 
                        dispatch ShowCounter
                )

            [ 
                [ nameof messageBusSubscription ], messageBusSubscription
            ]

        AvaloniaProgram.mkSimple init update bindings
        |> AvaloniaProgram.withSubscription subscriptions

    ElmishViewModel(program)