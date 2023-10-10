module AvaloniaXPlatExample.ViewModels.MainViewModel

open Elmish.Avalonia

type Model =
    {
        // ContentVM: IStart // TODO
        ContentVM: IElmishViewModel
    }

type Msg =
    | ShowAbout
    | ShowChart
    | ShowCounter
    | ShowFilePicker
    | ShowHome
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
    | ShowChart ->
        {model with ContentVM = ChartViewModel.vm}
    | ShowFilePicker ->
        {model with ContentVM = FilePickerViewModel.vm()}
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
    "ChartVM" |> Binding.oneWay (fun _ -> ChartViewModel.vm)
    "FilePickerVM" |> Binding.oneWay (fun _ -> FilePickerViewModel.vm())
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
