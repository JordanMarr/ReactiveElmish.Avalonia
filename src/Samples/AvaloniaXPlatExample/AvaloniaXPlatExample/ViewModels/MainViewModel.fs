module AvaloniaXPlatExample.ViewModels.MainViewModel

open Elmish.Avalonia

type Model =
    {
        ContentVM: IStart
    }

type Msg =
    | Msg

let init() =
    {
        ContentVM = CounterViewModel.vm
    }

let update (msg: Msg) (model: Model) =
    model

let bindings() : Binding<Model, Msg> list =
    [
    // Properties
    "ContentVM" |> Binding.oneWay (fun m -> m.ContentVM) ]

let designVM = ViewModel.designInstance (init()) (bindings())

let vm : IStart =
    ElmishViewModel(
            AvaloniaProgram.mkSimple init update bindings
            |> AvaloniaProgram.withElmishErrorHandler
                (fun msg exn ->
                    printfn $"ElmishErrorHandler: msg={msg}\n{exn.Message}\n{exn.StackTrace}"
                )
    )
