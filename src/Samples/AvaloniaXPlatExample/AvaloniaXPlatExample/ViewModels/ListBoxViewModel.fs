module AvaloniaXPlatExample.ViewModels.ListBoxViewModel

open System
open Elmish.Avalonia
open Elmish

type DNADoc = { Name : string ; Count : int}

type Model =
    {
        Greeting : string option
        Items: DNADoc list
        SelectedIndex : int option
    }

type Msg =
    | Delete
    | Add
    | ChangeGreeting of string
    | Clear
    | DoubleTapped
    | Selected of int

let init() =
    {   Greeting= None
        SelectedIndex = None
        Items = [
            {Name="Meredith" ; Count = 56}
            {Name="had" ; Count = 3}
            {Name="a" ; Count = 1}
            {Name="little" ; Count = 7}
            {Name="lamb" ; Count = 1000} ]
    },Cmd.none

let update (msg: Msg) (model: Model) =
    match msg with
    | Clear ->
        {model with Items = []},Cmd.none
    | Add ->
        match model.Greeting with
        | None ->
            {model with Greeting = Some "put some text here first"},Cmd.none
        | Some text ->
            {model with Items = {Name=text ; Count = 1}::model.Items ; Greeting = None},Cmd.none
    | ChangeGreeting v ->
        {model with Greeting = Some v},Cmd.none
    | Selected i ->
        {model with SelectedIndex = Some i ; Greeting = if i = -1 then None else Some $"picked {i}"},Cmd.none
    | DoubleTapped ->
        match model.SelectedIndex with
        | None ->
            {model with Greeting = Some $"choose an item"},Cmd.none
        | Some i ->
            {model with Greeting = Some $"doubletapped row {i}"},Cmd.none
    | Delete ->
        match model.SelectedIndex with
        | Some row when row > -1 ->
            {model with Items = model.Items |> List.removeAt row },Cmd.none
        | _ -> {model with Greeting = Some "Select an item first!"},Cmd.none

let bindings ()  : Binding<Model, Msg> list =
    [
        "Greeting" |> Binding.twoWay (get=(fun m ->  m.Greeting |> Option.defaultValue ""),set=(fun v m -> ChangeGreeting v))
        "Items" |> Binding.oneWay (fun m ->  m.Items)
        "SelectedIndex" |> Binding.twoWay(get=(fun m-> m.SelectedIndex |> Option.defaultValue -1),set=(fun v m -> Selected v))
        "DoubleTapped" |> Binding.cmd (fun m-> DoubleTapped)
        "Delete" |> Binding.cmd Delete
        "Clear" |> Binding.cmd Clear
        "Add" |> Binding.cmd Add
    ]

let designVM = ViewModel.designInstance (fst (init())) (bindings())

let vm = ElmishViewModel(AvaloniaProgram.mkProgram init update bindings
            |> AvaloniaProgram.withElmishErrorHandler
                (fun msg exn ->
                    printfn $"ElmishErrorHandlerCV: msg={msg}\n{exn.Message}\n{exn.StackTrace}"
                )
        )

