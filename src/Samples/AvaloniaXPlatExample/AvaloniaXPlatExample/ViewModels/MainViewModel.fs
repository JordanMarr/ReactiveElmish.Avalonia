module AvaloniaXPlatExample.ViewModels.MainViewModel

open Elmish.Avalonia

type Model =
    {
        ContentVM: IStart
    }

type Msg =
    | Msg
    | ShowHome
    | ShowCounter
    | ShowListBox

let init() =
    {
        // ContentVM = CounterViewModel.vm
        ContentVM = ListBoxViewModel.vm
    }

let rec update (msg: Msg) (model: Model) =
    match msg with
    | ShowHome ->
        {model with ContentVM = vm}
    | ShowCounter ->
        {model with ContentVM = CounterViewModel.vm}
    | ShowListBox ->
        {model with ContentVM = ListBoxViewModel.vm}

and bindings() : Binding<Model, Msg> list =
    [
    // Properties
    "CounterVM" |> Binding.oneWay (fun m -> CounterViewModel.vm)
    "ListBoxVM" |> Binding.oneWay (fun m -> ListBoxViewModel.vm)
    ]

and designVM = ViewModel.designInstance (init()) (bindings())

and vm : IStart =
    ElmishViewModel(
            AvaloniaProgram.mkSimple init update bindings
            |> AvaloniaProgram.withElmishErrorHandler
                (fun msg exn ->
                    printfn $"ElmishErrorHandler: msg={msg}\n{exn.Message}\n{exn.StackTrace}"
                )
    )
