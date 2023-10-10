module AvaloniaXPlatExample.ViewModels.MainViewModel

open Elmish.Avalonia

type Model =
    {
        // ContentVM: IStart // TODO
        ContentVM: IElmishViewModel
    }

type Msg =
    | ShowAbout
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
    | ShowAbout ->
        {model with ContentVM = AboutViewModel.vm}
    | ShowHome ->
        {model with ContentVM = vm}
    | ShowCounter ->
        {model with ContentVM = CounterViewModel.vm}
    | ShowListBox ->
        {model with ContentVM = ListBoxViewModel.vm}

and bindings() : Binding<Model, Msg> list =
    [
    // Properties
    "AboutVM" |> Binding.oneWay (fun _ -> AboutViewModel.vm)
    "CounterVM" |> Binding.oneWay (fun _ -> CounterViewModel.vm)
    "ListBoxVM" |> Binding.oneWay (fun _ -> ListBoxViewModel.vm)
    ]

and designVM = ViewModel.designInstance (init()) (bindings())

and vm : IElmishViewModel =
    ElmishViewModel(
            AvaloniaProgram.mkSimple init update bindings
            |> AvaloniaProgram.withElmishErrorHandler
                (fun msg exn ->
                    printfn $"ElmishErrorHandler: msg={msg}\n{exn.Message}\n{exn.StackTrace}"
                )
    )
