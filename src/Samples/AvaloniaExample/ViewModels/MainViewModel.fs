module AvaloniaExample.ViewModels.MainViewModel

open Elmish.Avalonia
open Elmish

type Model = 
    {
        CounterVM: IElmishViewModel
        AboutVM: IElmishViewModel
        ContentVM: IElmishViewModel
        CurrentCount: int
    }

type Msg = 
    | ShowCounter
    | ShowAbout
    | SetCurrentCount of int

let init() = 
    { 
        CounterVM = CounterViewModel.vm
        AboutVM = AboutViewModel.vm 0
        ContentVM = CounterViewModel.vm
        CurrentCount = 0
    }

let update (msg: Msg) (model: Model) = 
    match msg with
    | ShowCounter -> 
        { model with ContentVM = CounterViewModel.vm }
    | ShowAbout ->
        { model with ContentVM = AboutViewModel.vm model.CurrentCount }
    | SetCurrentCount count ->
        { model with CurrentCount = count }

let bindings() : Binding<Model, Msg> list = [ 
    // Properties
    "ContentVM" |> Binding.oneWay (fun m -> m.ContentVM)
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
                        dispatch ShowCounter
                    | Messaging.GlobalMsg.SetCount count ->
                        dispatch (SetCurrentCount count)
                )

            [ 
                [ nameof messageBusSubscription ], messageBusSubscription
            ]

        AvaloniaProgram.mkSimple init update bindings
        |> AvaloniaProgram.withSubscription subscriptions

    ElmishViewModel(program)